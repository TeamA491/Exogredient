<template>
    <div>
        ingredient view
        <v-btn @click="view">see ingreditnes</v-btn>
       <div v-for="ingredient in ingredients" :key=ingredient.ingredientName>
                <p>{{ingredient.ingredientName}} from {{storeViewData.storeName}}</p>
                <p>Posted On: {{ingredient.postTimeDate}}</p>                                  
                <img :src="ingredient.photo" height="1080px" width="720px" />
                <p>Price: {{ingredient.price}}</p>
                <p>Rating:{{ingredient.rating}}</p>
                <p>Uploaded by: {{ingredient.uploader}}</p>
                <p>Description: {{ingredient.Description}}</p>
       </div>
    </div>

</template>

<script>
import * as global from '../globalExports.js';

export default { 
    data (){
        return {
            ingredients:[] 
        }
    },
    methods: {
        view: function(){
                        fetch(`${global.ApiDomainName}/api/IngredientView/GetIngredients?ingredientName=beef&storeId=1`)
            .then((response) =>
            {
                // error if(!respone.statuscode.ok) thrwo er
                return response.json();
            })
            .then((data)=> {
                for(let index in data){
                    this.ingredients.push(data[index])
                }

            })
        }
    },
    computed:{
        storeViewData: function () {
            return this.$store.state.storeViewData;
        }
    },
    created () {
        this.view();
    }
}

</script>