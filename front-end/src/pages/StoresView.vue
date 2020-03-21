<template>
    <div id = "stores-table">
        <table class = "table is-fullwidth">
            <tr>
                <th>Store Name</th>
                <th>Number of ingredients</th>
                <th>Distance</th>
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
    name: "StoresView",
    data(){
        return{

        }
    },
    computed:{
        stores: function(){
            return this.$store.getters.storeResults;
        }
    },
    methods:{
        displayIngredientsView: async function(storeId){
            let response = await fetch(`https://localhost:5001/api/search/storeView?` +
            `username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}&failureCounter=0&storeId=${storeId}&pagination=1&ingredientName=${this.$store.state.ingredientName}`);
            let responseJson = await response.json();
            let ingredients = responseJson.item2;
            let storeViewData = responseJson.item1;
            storeViewData.storeId = storeId;
            console.log(ingredients);
            console.log(storeViewData);
            this.$store.dispatch('updateIngredientResults',ingredients);
            this.$store.dispatch('updateStoreViewData', storeViewData);
            this.$router.push("/IngredientsView");
        }
    }
}
</script>