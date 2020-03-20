<template>
    <div>
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
    </div>
</template>>

<script>
export default {
    name: "IngredientsView",
    computed:{
        ingredients: function(){
            return this.$store.getters.ingredientResults;
        },
        storeViewData: function(){
            return this.$store.state.storeViewData;
        },
        storeImage: function(){
            return `https://localhost:5001/api/search/image?id=${this.storeViewData.storeId}`;
        }
    },
    methods:{
        displayIngredientView: function(){
            //TODO: Implement what to do when user clicks the ingredient.
        },
        openMap: function(){
            window.open(`https://www.google.com/maps/search/?api=1&query=${this.storeViewData.latitude},${this.storeViewData.longitude}&query_place_id=${this.storeViewData.placeId}`, "_blank");
        }
    }
}
</script>