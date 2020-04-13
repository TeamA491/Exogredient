<template>
    <div id="storeView">
        <p><a @click="openMap">{{storeViewData.storeName}}</a></p>
        <img :src="storeImage" width="100" height="100">
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
import {resultsPerPage} from '../main.js'
export default {
    name: "StoreView",
    mounted(){
        let currentPage = this.$store.state.currentPages.storeView;
        this.$refs.paginations[currentPage-1].classList.add("is-current");
    },
    computed:{
        resultsPerPage: function(){
            return resultsPerPage;
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
            return `https://localhost:5001/api/search/image?id=${this.storeViewData.storeId}`;
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
            var encodedOriginAddress = encodeURIComponent(this.$store.state.searchData.address);
            window.open(`https://www.google.com/maps/dir/?api=1&origin=${encodedOriginAddress}&destination=${this.storeViewData.storeName}&destination_place_id=${this.storeViewData.placeId}&travelmode=driving`, "_blank");
        },
        updatePagination: function(newPage){
            this.$refs.paginations[this.currentPage-1].classList.remove("is-current");
            this.$refs.paginations[newPage-1].classList.add("is-current");
            this.$store.dispatch('updateStoreViewCurrentPage', newPage);
        },
        movePage: async function(newPage){
            let skipPages = newPage - this.currentPage;
            if(skipPages === 0){
                return;
            }
            let lastIngredientName = this.ingredients[this.ingredients.length-1].ingredientName;
            let ingredientResultsResponse = await fetch(`https://localhost:5001/api/search/getIngredientResults?` +
            `username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}&storeId=${this.$store.state.storeViewData.storeId}&skipPages=${skipPages}`
            + `&lastPageResultsNum=${this.ingredients.length}&lastIngredientName=${lastIngredientName}`
            + `&ingredientName=${this.$store.state.searchData.searchBy === 'ingredient'? this.$store.state.searchData.searchTerm:""}`);

            let ingredientResults = await ingredientResultsResponse.json();
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