<template>
    <div id="storeView">
        <p><a @click="openMap">{{storeViewData.storeName}}</a></p>
        <img :src="storeImage" @error="storeImageLoadError=true" width="100" height="100">
        <p>{{storeViewData.storeDescription}}</p>
        <table class = "table is-fullwidth">
            <tr>
                <th>Ingredient Name</th>
                <th>Average Price</th>
                <th>Uploads Number</th>
            </tr>
            <tr v-for="ingredient in ingredients" :key=ingredient.ingredientName>
                <td><a @click="displayIngredientView">{{ingredient.ingredientName}}</a></td>
                <td>{{ingredient.averagePrice}}</td>
                <td>{{ingredient.uploadNum}}</td>
            </tr>
        </table>
        <nav class='pagination'>
            <button class='pagination-previous' @click="movePage(currentPage-1)" :disabled="isPreviousButtonDisabled">Previous</button>
            <ul class='pagination-list'>
                <li v-for="i in Math.ceil(ingredientResultsTotalNum/resultsPerPage)" :key=i>
                    <a class='pagination-link' ref="paginations" @click="movePage(i)">{{i}}</a>
                </li>
            </ul>
            <button class='pagination-next' @click="movePage(currentPage+1)" :disabled="isNextButtonDisabled">Next</button>
        </nav>        
    </div>
</template>>

<script>
import * as global from '../globalExports.js';
import defaultStoreImage from "../assets/defaultStoreImage.jpeg";

export default {
    name: "StoreView",
    mounted(){
        let currentPage = this.$store.state.currentPages.storeView;
        this.$refs.paginations[currentPage-1].classList.add("is-current");
    },
    data(){
        return{
            storeImageLoadError: false,
        }
    },
    computed:{
        resultsPerPage: function(){
            return global.ResultsPerPage;
        },

        ingredients: function(){
            return this.$store.getters.ingredientResults;
        },

        ingredientResultsTotalNum: function(){
            return this.$store.state.totalResultsNum.ingredientResultsTotalNum;
        },
        
        storeViewData: function(){
            return this.$store.state.storeViewData;
        },

        storeImage: function(){
            var storeImage =`${global.ApiDomainName}/api/storeImage?`
                + `storeId=${this.storeViewData.storeId}&username=${this.$store.state.userData.username}`
                + `&ipAddress=${this.$store.state.userData.ipAddress}`;

            return this.$data.storeImageLoadError? defaultStoreImage : storeImage;
        },

        currentPage: function(){
            return this.$store.state.currentPages.storeView;
        },

        isPreviousButtonDisabled: function(){
            if(this.currentPage === 1){
                return true;
            }
            return false;
        },

        isNextButtonDisabled: function(){
            if(this.currentPage === Math.ceil(this.ingredientResultsTotalNum/this.resultsPerPage)){
                return true;
            }
            return false;
        }
    },
    methods:{

        displayIngredientView: function(){
            //TODO: Implement what to do when user clicks the ingredient.
        },

        openMap: function(){
            // Opens Google Maps direction to the store from the center of search.
            var encodedOriginAddress = encodeURIComponent(this.$store.state.searchData.address);
            window.open(`https://www.google.com/maps/dir/?api=1`
                + `&origin=${encodedOriginAddress}&destination=${this.storeViewData.storeName}`
                + `&destination_place_id=${this.storeViewData.placeId}&travelmode=driving`, "_blank");
        },

        updatePagination: function(newPage){
            // Remove "is-current" class from the anchor of the current page.
            this.$refs.paginations[this.currentPage-1].classList.remove("is-current");
            // Add "is-current" class to the anchor of the new page.
            this.$refs.paginations[newPage-1].classList.add("is-current");
            // Update SearchResultsView's current page in Vuex.
            this.$store.dispatch('updateStoreViewCurrentPage', newPage);
        },

        movePage: async function(newPage){
            // Compute how many pages are being skipped.
            let skipPages = newPage - this.currentPage;

            // Check if the current page is clicked.
            if(skipPages === 0){
                return;
            }
            
            // Get the ingredient name of the last store in this current page.
            let lastIngredientName = this.ingredients[this.ingredients.length-1].ingredientName;

            // Fetch list of ingredients for the new page.
            let ingredientResultsResponse = 
                await fetch(`${global.ApiDomainName}/api/getIngredientResults?` 
                + `username=${this.$store.state.userData.username}&ipAddress=${this.$store.state.userData.ipAddress}`
                + `&storeId=${this.$store.state.storeViewData.storeId}&skipPages=${skipPages}`
                + `&lastPageResultsNum=${this.ingredients.length}&lastIngredientName=${lastIngredientName}`
                + `&ingredientName=${this.$store.state.searchData.searchBy === global.SearchByIngredient?
                 this.$store.state.searchData.searchTerm:""}`);
        
            // Handle any error from the HTTP status code.
            global.ErrorHandler(this.$router, ingredientResultsResponse);

            // Get the list of ingredients.
            let ingredientResults = await ingredientResultsResponse.json();
            
            // Update the list of ingredients in Vuex.
            this.$store.dispatch('updateIngredientResults', ingredientResults);
            
            this.updatePagination(newPage);
        },
    }
}
</script>

<style>
#storeView {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50;
}
</style>