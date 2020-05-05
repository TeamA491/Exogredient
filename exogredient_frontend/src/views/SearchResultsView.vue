<template>
    <div id = "searchResultsView">
        <NoResultsView v-if="storeResultsTotalNum===0"></NoResultsView>
        <div v-if="storeResultsTotalNum!==0">
            <p>Sorted by: <strong>{{sortOption.by === SortByIngredientNum? 'number of ingredients':sortOption.by}} 
                {{sortOption.fromSmallest? "(ascending)":"(descending)"}}</strong></p>
            <table class = "table is-fullwidth">
                <tr>
                    <th>Store Name</th>
                    <th><a @click="sortByIngredientNum">Number of ingredients &#8693;</a></th>
                    <th><a @click="sortByDistance">Distance &#8693;</a></th>
                </tr>
                <tr v-for="store in stores" :key=store.storeId>
                    <td><a @click="displayStoreView(store.storeId)">{{store.storeName}}</a></td>
                    <td>{{store.ingredientNum}}</td>
                    <td>{{Number(Math.round(store.distance+'e2')+'e-2')}} mile</td>
                </tr>
            </table>
            <nav class='pagination'>
                <button class='pagination-previous' @click="movePage(currentPage-1)" :disabled="isPreviousButtonDisabled">Previous</button>
                <ul class='pagination-list'>
                    <li v-for="i in Math.ceil(storeResultsTotalNum/resultsPerPage)" :key=i>
                        <a class='pagination-link' ref="paginations" @click="movePage(i)">{{i}}</a>
                    </li>
                </ul>
                <button class='pagination-next' @click="movePage(currentPage+1)" :disabled="isNextButtonDisabled">Next</button>
            </nav>
        </div>
    </div>
</template>

<script>
import * as global from "../globalExports.js";
import NoResultsView from "./NoResultsView.vue";
export default {
    name: "SearchResultsView",
    components:{
        NoResultsView
    },
    mounted(){
        let currentPage = this.$store.state.currentPages.searchResultsView;
        this.$refs.paginations[currentPage-1].classList.add("is-current");
        global.Bus.$on("updatePagination",this.updatePagination);
    },
    data(){
        return{
            SortByIngredientNum: global.SortByIngredientNum
        }
    },
    computed:{
        sortOption: function(){
            return this.$store.state.sortOption;
        },

        resultsPerPage: function(){
            return global.ResultsPerPage;
        },

        stores: function(){
            return this.$store.getters.storeResults;
        },

        currentPage: function(){
            return this.$store.state.currentPages.searchResultsView;
        },

        storeResultsTotalNum: function(){
            return this.$store.state.totalResultsNum.storeResultsTotalNum;
        },

        isPreviousButtonDisabled: function(){
            if(this.currentPage === 1){
                return true;
            }
            return false;
        },

        isNextButtonDisabled: function(){
            if(this.currentPage === Math.ceil(this.storeResultsTotalNum/this.resultsPerPage)){
                return true;
            }
            return false;
        }
    },
    methods:{
        updateStoreResults: async function(searchTerm,latitude,longitude,
            radius,searchBy,lastStoreData,lastStoreId,lastPageResultsNum,
            skipPages,sortOption,fromSmallest,username,ipAddress)
        {
            // Fetch search results of the given page and sort method.
            let resultsResponse = 
                    await fetch(`${global.ApiDomainName}/api/getStoreResults?`
                    + `searchTerm=${searchTerm}&latitude=${latitude}`
                    + `&longitude=${longitude}&radius=${radius}`
                    + `&searchBy=${searchBy}&lastStoreData=${lastStoreData}`
                    + `&lastStoreId=${lastStoreId}&lastPageResultsNum=${lastPageResultsNum}`
                    + `&skipPages=${skipPages}&sortOption=${sortOption}&fromSmallest=${fromSmallest}`
                    + `&username=${username}&ipAddress=${ipAddress}`);

            // Handle any error from the HTTP status code. 
            global.ErrorHandler(this.$router, resultsResponse);

            // Get the list of search results and update Vuex.
            let stores = await resultsResponse.json();
            this.$store.dispatch('updateStoreResults',stores);
        },

        updatePagination: function(newPage){
            // Remove "is-current" class from the anchor of the current page.
            this.$refs.paginations[this.currentPage-1].classList.remove("is-current");
            // Add "is-current" class to the anchor of the new page.
            this.$refs.paginations[newPage-1].classList.add("is-current");
            // Update SearchResultsView's current page in Vuex.
            this.$store.dispatch('updateSearchResultsViewCurrentPage', newPage);
        },

        displayStoreView: async function(storeId){
            // Fetch the list of ingredients of the store.
            let ingredientResultsResponse = 
                await fetch(`${global.ApiDomainName}/api/getIngredientResults?` +
                `username=${this.$store.state.userData.username}&ipAddress=${this.$store.state.userData.ipAddress}`
                + `&storeId=${storeId}&skipPages=${global.DefaultSkipPages}&lastIngredientName=`
                + `&ingredientName=${this.$store.state.searchData.searchBy === global.SearchByIngredient? 
                this.$store.state.searchData.searchTerm:""}`);
            
            // Handle any error from the HTTP status code.
            global.ErrorHandler(this.$router, ingredientResultsResponse);

            // Fetch StoreView data and the number of total ingredients of the store.
            let storeViewDataResponse = 
                await fetch(`${global.ApiDomainName}/api/storeViewData?storeId=${storeId}`
                + `&username=${this.$store.state.userData.username}&ipAddress=${this.$store.state.userData.ipAddress}`
                + `&ingredientName=${this.$store.state.searchData.searchBy === global.SearchByIngredient? 
                this.$store.state.searchData.searchTerm:""}`);

            // Handle any error from the HTTP status code.
            global.ErrorHandler(this.$router, storeViewDataResponse);

            // Get the list of ingredients and StoreView data & total ingredients number.
            let ingredientResults = await ingredientResultsResponse.json();
            let storeViewDataTuple = await storeViewDataResponse.json();

            // Extract StoteView data and total ingredients number.
            let storeViewData = storeViewDataTuple.item2;
            let ingredientResultsTotalNum = storeViewDataTuple.item1;
            
            // Update list of ingredients, StoreView data, 
            // total ingredients number, and StoreView current page in Vuex.
            this.$store.dispatch('updateIngredientResults',ingredientResults);
            this.$store.dispatch('updateStoreViewData', storeViewData);
            this.$store.dispatch('updateIngredientResultsTotalNum', ingredientResultsTotalNum);
            this.$store.dispatch('updateStoreViewCurrentPage',  global.DefaultPage);

            // Route to StoreView.
            this.$router.push("/store");
        },

        movePage: async function(newPage){
            // Compute how many pages are being skipped.
            let skipPages = newPage - this.currentPage;

            // Check if the current page is clicked.
            if(skipPages === 0){
                return;
            }

            // Get the storeID of the last store in this current page.
            let lastStoreId = this.stores[this.stores.length-1].storeId;

            // Get this current page's total results number.
            let lastPageResultsNum = this.stores.length;

            // Get the search data.
            let searchData = this.$store.state.searchData;

            // Get the data of the last store in this current page 
            // (distance OR number of ingredients).
            let lastStoreData = 
                this.sortOption.by === global.SortByDistance? 
                this.stores[this.stores.length-1].distance 
                :this.stores[this.stores.length-1].ingredientNum;

            // Call method to update store results for the new page.
            await this.updateStoreResults(
                searchData.searchTerm, searchData.lat, searchData.lng, 
                searchData.radius, searchData.searchBy,lastStoreData,lastStoreId,
                lastPageResultsNum,skipPages,this.sortOption.by,this.sortOption.fromSmallest,
                this.$store.state.userData.username,this.$store.state.userData.ipAddress);

            // Call method to update pagination.
            this.updatePagination(newPage);
        },

        sortByIngredientNum: async function(){
            if(this.sortOption.by !== global.SortByIngredientNum){
                // If not already sorted by number of ingredients, 
                // sort by number of ingredients in descending order.
                this.$store.dispatch('updateSortOption', 
                {by:global.SortByIngredientNum, fromSmallest: false});
            }else{
                // If already sorted by number of ingredients,
                // change the sort order to opposite. 
                this.$store.dispatch('updateSortOption', 
                {by:global.SortByIngredientNum, fromSmallest: !this.sortOption.fromSmallest});
            }

            if(this.storeResultsTotalNum <= this.resultsPerPage){
                // If there is only one page, sort in Vuex.
                this.$store.dispatch('sortStoreResults',this.sortOption);
            }else{
                // If there is more than one page,
                // get new sorted results from the server.  
                let searchData = this.$store.state.searchData;
                await this.updateStoreResults(
                    searchData.searchTerm, searchData.lat, searchData.lng, searchData.radius, 
                    searchData.searchBy, global.NoLastStoreData,global.NoLastStoreId,
                    global.NoLastPageResultsNum,global.DefaultSkipPages,this.sortOption.by,
                    this.sortOption.fromSmallest,this.$store.state.userData.username,
                    this.$store.state.userData.ipAddress);
            }
            this.updatePagination(global.DefaultPage);
            
        },

        sortByDistance: async function(){
            if(this.sortOption.by !== global.SortByDistance){
                // If not already sorted by distance, 
                // sort by distance in ascending order.
                this.$store.dispatch('updateSortOption', 
                {by:global.SortByDistance, fromSmallest: true});
            }else{
                // If already sorted by distance,
                // change the sort order to opposite. 
                this.$store.dispatch('updateSortOption', 
                {by:global.SortByDistance, fromSmallest: !this.sortOption.fromSmallest});
            }
            if(this.storeResultsTotalNum <= this.resultsPerPage){
                // If there is only one page, sort in Vuex.
                this.$store.dispatch('sortStoreResults',this.sortOption);
            }else{
                // If there is more than one page,
                // get new sorted results from the server.  
                let searchData = this.$store.state.searchData;
                await this.updateStoreResults(
                    searchData.searchTerm, searchData.lat, searchData.lng, searchData.radius, 
                    searchData.searchBy, global.NoLastStoreData,global.NoLastStoreId,
                    global.NoLastPageResultsNum,global.DefaultSkipPages,this.sortOption.by,
                    this.sortOption.fromSmallest,this.$store.state.userData.username,
                    this.$store.state.userData.ipAddress);
            }
            this.updatePagination(global.DefaultPage);
        }
    }
}
</script>

<style>
#searchResultsView {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50;
}
</style>