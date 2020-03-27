<template>
    <div id = "stores-table">
        <p>Sorted by: {{sortOption.by}}</p>
        <table class = "table is-fullwidth">
            <tr>
                <th>Store Name</th>
                <th><a @click="sortByIngredientNum">Number of ingredients</a></th>
                <th><a @click="sortByDistance">Distance</a></th>
            </tr>
            <tr v-for="store in stores" :key=store.storeId>
                <td><a @click="displayIngredientsView(store.storeId)">{{store.storeName}}</a></td>
                <td>{{store.ingredientNum}}</td>
                <td>{{store.distance}} mile</td>
            </tr>
        </table>

    </div>
</template>

<script>
export default {
    name: "SearchResultsView",
    data(){
        return{
            sortOption:{by:'distance', fromSmallest: true}
        }
    },
    computed:{
        stores: function(){
            return this.$store.getters.storeResults;
        },
        updateSortOption: function(){
            this.$data.sortOption = this.$store.state.sortOption;
        }
    },
    methods:{
        displayIngredientsView: async function(storeId){
            let response = await fetch(`https://localhost:5001/api/search/storeView?` +
            `username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}&failureCounter=0&storeId=${storeId}&lastIngredientName=`
            + `&ingredientName=${this.$store.state.searchData.searchBy === 'ingredient'? this.$store.state.searchData.searchTerm:""}`);
            let responseJson = await response.json();
            let ingredients = responseJson.item2;
            let storeViewData = responseJson.item1;
            storeViewData.storeId = storeId;
            console.log(ingredients);
            console.log(storeViewData);
            this.$store.dispatch('updateIngredientResults',ingredients);
            this.$store.dispatch('updateStoreViewData', storeViewData);
            this.$router.push("/StoreView");
        },
        sortByIngredientNum: async function(){
            if(this.$data.sortOption.by !== 'ingredientNum'){
                this.$data.sortOption.by = 'ingredientNum';
                this.$data.sortOption.fromSmallest = false;
            }else{
                this.$data.sortOption.fromSmallest = !this.$data.sortOption.fromSmallest;
            }

            if(this.$store.state.totalResultsNum <= 100){
                this.$store.dispatch('sortStoreResults',this.$data.sortOption);
            }else{
                let resultsResponse = 
                    await fetch(`https://localhost:5001/api/search/getResults?`
                    + `searchTerm=${this.searchData.searchTerm}&latitude=${this.searchData.lat}&longitude=${this.searchData.lng}&radius=${this.searchData.radius}`
                    + `&searchBy=${this.searchData.searchBy}&lastStoreData=${-1}&lastStoreId=${0}&sortOption=${this.$data.sortOption.by}&fromSmallest=${this.$data.sortOption.fromSmallest}`
                    + `&failureCount=${0}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`);
                let stores = await resultsResponse.json();
                this.$store.dispatch('updateStoreResults',stores);
            }
            
        },

        sortByDistance: async function(){
            if(this.$data.sortOption.by !== 'distance'){
                this.$data.sortOption.by = 'distance';
                this.$data.sortOption.fromSmallest = true;
            }else{
                this.$data.sortOption.fromSmallest = !this.$data.sortOption.fromSmallest;
            }
            if(this.$store.state.totalResultsNum <= 100){
                this.$store.dispatch('sortStoreResults',this.$data.sortOption);
            }else{
                let resultsResponse = 
                    await fetch(`https://localhost:5001/api/search/getResults?`
                    + `searchTerm=${this.searchData.searchTerm}&latitude=${this.searchData.lat}&longitude=${this.searchData.lng}&radius=${this.searchData.radius}`
                    + `&searchBy=${this.searchData.searchBy}&lastStoreData=${-1}&lastStoreId=${0}&sortOption=${this.$data.sortOption.by}&fromSmallest=${this.$data.sortOption.fromSmallest}`
                    + `&failureCount=${0}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`);
                let stores = await resultsResponse.json();
                this.$store.dispatch('updateStoreResults',stores);
            }
        }
    }
}
</script>