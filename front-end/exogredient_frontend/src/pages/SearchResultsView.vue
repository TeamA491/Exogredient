<template>
    <div id = "searchResultsView">
        <NoResults v-if="storeResultsTotalNum===0"></NoResults>
        <div v-if="storeResultsTotalNum!==0">
            <p>Sorted by: <strong>{{sortOption.by}} {{sortOption.fromSmallest? "(ascending)":"(descending)"}}</strong></p>
            <table class = "table is-fullwidth">
                <tr>
                    <th>Store Name</th>
                    <th><a @click="sortByIngredientNum">Number of ingredients</a></th>
                    <th><a @click="sortByDistance">Distance</a></th>
                </tr>
                <tr v-for="store in stores" :key=store.storeId>
                    <td><a @click="displayIngredientsView(store.storeId)">{{store.storeName}}</a></td>
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
import {bus, resultsPerPage} from "../main.js";
import NoResults from "../pages/NoResults.vue";
export default {
    name: "SearchResultsView",
    components:{
        NoResults
    },
    mounted(){
        let currentPage = this.$store.state.currentPages.searchResultsView;
        this.$refs.paginations[currentPage-1].classList.add("is-current");
        bus.$on("updatePagination",this.updatePagination);
    },
    computed:{
        sortOption: function(){
            return this.$store.state.sortOption;
        },
        resultsPerPage: function(){
            return resultsPerPage;
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
        updateStoreResults: async function(searchTerm,latitude,longitude,radius,searchBy,lastStoreData,lastStoreId,
            lastPageResultsNum,skipPages,sortOption,fromSmallest,username,ipAddress){
            let resultsResponse = 
                    await fetch(`https://localhost:5001/api/search/getStoreResults?`
                    + `searchTerm=${searchTerm}&latitude=${latitude}&longitude=${longitude}&radius=${radius}`
                    + `&searchBy=${searchBy}&lastStoreData=${lastStoreData}&lastStoreId=${lastStoreId}&lastPageResultsNum=${lastPageResultsNum}`
                    + `&skipPages=${skipPages}&sortOption=${sortOption}&fromSmallest=${fromSmallest}`
                    + `&username=${username}&ipAddress=${ipAddress}`);
            let stores = await resultsResponse.json();
            this.$store.dispatch('updateStoreResults',stores);
        },
        updatePagination: function(newPage){
            this.$refs.paginations[this.currentPage-1].classList.remove("is-current");
            this.$refs.paginations[newPage-1].classList.add("is-current");
            this.$store.dispatch('updateSearchResultsViewCurrentPage', newPage);
        },
        displayIngredientsView: async function(storeId){
            let ingredientResultsResponse = await fetch(`https://localhost:5001/api/search/getIngredientResults?` +
            `username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}&storeId=${storeId}&skipPages=1&lastIngredientName=`
            + `&ingredientName=${this.$store.state.searchData.searchBy === 'ingredient'? this.$store.state.searchData.searchTerm:""}`);

            let storeViewDataResponse = await fetch(`https://localhost:5001/api/search/storeViewData?storeId=${storeId}`
            + `&ingredientName=${this.$store.state.searchData.searchBy === 'ingredient'? this.$store.state.searchData.searchTerm:""}`);

            let ingredientResults = await ingredientResultsResponse.json();
            let storeViewDataTuple = await storeViewDataResponse.json();

            let storeViewData = storeViewDataTuple.item2;
            let ingredientResultsTotalNum = storeViewDataTuple.item1;
            
            // storeViewData.storeId = storeId;
            console.log(ingredientResults);
            console.log(storeViewData);
            this.$store.dispatch('updateIngredientResults',ingredientResults);
            this.$store.dispatch('updateStoreViewData', storeViewData);
            this.$store.dispatch('updateIngredientResultsTotalNum', ingredientResultsTotalNum);
            this.$store.dispatch('updateStoreViewCurrentPage',1)
            this.$router.push("/StoreView");
        },
        movePage: async function(newPage){
            let skipPages = newPage - this.currentPage;
            if(skipPages === 0){
                return;
            }
            let searchData = this.$store.state.searchData;
            let lastStoreData = 
                this.sortOption.by === 'distance'? this.stores[this.stores.length-1].distance : this.stores[this.stores.length-1].ingredientNum;
            let lastStoreId = this.stores[this.stores.length-1].storeId;
            let lastPageResultsNum = this.stores.length;
            await this.updateStoreResults(searchData.searchTerm, searchData.lat, searchData.lng, searchData.radius, searchData.searchBy, 
                                    lastStoreData,lastStoreId,lastPageResultsNum,skipPages,this.sortOption.by,
                                    this.sortOption.fromSmallest,this.$store.state.username,this.$store.state.ipAddress);

            this.updatePagination(newPage);
        },

        sortByIngredientNum: async function(){
            if(this.sortOption.by !== 'ingredientNum'){
                this.$store.dispatch('updateSortOption', {by:'ingredientNum', fromSmallest: false});
            }else{
                this.$store.dispatch('updateSortOption', {by:'ingredientNum', fromSmallest: !this.sortOption.fromSmallest});
            }

            if(this.storeResultsTotalNum <= resultsPerPage){
                this.$store.dispatch('sortStoreResults',this.sortOption);
            }else{
                let searchData = this.$store.state.searchData;

                await this.updateStoreResults(searchData.searchTerm, searchData.lat, searchData.lng, searchData.radius, searchData.searchBy, 
                                        -1,0,0,1,this.sortOption.by,this.sortOption.fromSmallest,
                                        this.$store.state.username,this.$store.state.ipAddress);
            }
            this.updatePagination(1);
            
        },

        sortByDistance: async function(){
            if(this.sortOption.by !== 'distance'){
                this.$store.dispatch('updateSortOption', {by:'distance', fromSmallest: true});
            }else{
                this.$store.dispatch('updateSortOption', {by:'distance', fromSmallest: !this.sortOption.fromSmallest});
            }
            if(this.storeResultsTotalNum <= resultsPerPage){
                this.$store.dispatch('sortStoreResults',this.sortOption);
            }else{
                let searchData = this.$store.state.searchData;
                await this.updateStoreResults(searchData.searchTerm, searchData.lat, searchData.lng, searchData.radius, searchData.searchBy, 
                                        -1,0,0,1,this.sortOption.by,this.sortOption.fromSmallest,
                                        this.$store.state.username,this.$store.state.ipAddress);
            }
            this.updatePagination(1);
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