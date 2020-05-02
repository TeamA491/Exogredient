<template>
    <div>
        <h1>HELLO WORLD</h1>
        ingredient view
        <v-btn @click="view">see ingreditnes</v-btn>
       <div v-for="ingredient in ingredients" :key=ingredient.ingredientName>
                <p>{{ingredient.ingredientName}} from {{storeViewData.storeName}}</p>
                <p>Posted On: {{ingredient.postTimeDate}}</p>    
                <v-btn @click="undoUpvoteStatus ? undoUpvote(ingredient.uploadId) : upvote(ingredient.uploadId)">Upvotes: {{ingredient.upvote}}</v-btn>
                <v-btn @click="undoDownvoteStatus ? undoDownvote(ingredient.uploadId) : downvote(ingredient.uploadId)">Downvotes: {{ingredient.downvote}}</v-btn>                              
                <img :src="ingredient.photo" />
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
            undoUpvoteStatus: false,
            undoDownvoteStatus: false,

            ingredients:[],
            ingredientsPageLength: 1,
            ingredientsStatus: false,
            ingredientsPage: 1,

        }
    },
    computed:{
        storeViewData: function () {
            return this.$store.state.storeViewData;
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
        },
        upvote: function(uploadId) {
            fetch(`${global.ApiDomainName}/api/IngredientView/Upvote?uploadId=${uploadId}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`,
            {
                method:"POST",
                mode:"cors"
            })
            .then((response) =>
            {
                
            });
            this.undoUpvoteStatus = true;
       },
        downvote: function(uploadId) {
             fetch(`${global.ApiDomainName}/api/IngredientView/Downvote?uploadId=${uploadId}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`,
            {
                method:"POST",
                mode:"cors"
            })
            .then((response) =>
            {
                
            });
            this.undoDownvoteStatus = true;
        },
        undoUpvote: function(uploadId) {
            fetch(`${global.ApiDomainName}/api/IngredientView/UndoUpvote?uploadId=${uploadId}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`,
            {
                method:"POST",
                mode:"cors"
            })
            .then((response) =>
            {
                
            });
            this.undoUpvoteStatus = false;
       },
       undoDownvote: function(uploadId) {
             fetch(`${global.ApiDomainName}/api/IngredientView/UndoDownvote?uploadId=${uploadId}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`,
            {
                method:"POST",
                mode:"cors"
            })
            .then((response) =>
            {
                
            });
            this.undoDownvoteStatus = false;
        },
        ingredientsViewPage: function(newPage) {
            // Call IngredientsView for new pagination
            fetch(`${global.ApiDomainName}/api/IngredientView/GetIngredients?ingredientName=${this.$store.state.ingredientsList[0].ingredientName}&storeId=${this.$store.state.ingredientsList[0].storeId}&pagination=${newPage - 1}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`)
            .then((response) =>{
                global.ErrorHandler(this.$router, response);
                return  response.json();
            })
            .then((data) => {
                data.forEach((i) =>{
                    this.ingredients.push(i);
                });
            });
            // Get pagination size.
            fetch(`${global.ApiDomainName}/api/IngredientView/GetTotalIngredientsNumber?storeId=${this.$store.state.storeId}&ingredientName=${this.$store.state.ingredientsList[0].ingredientName}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`)
            .then((response) => {
                global.ErrorHandler(this.$router,response);
                return response.json()
            })
            .then((data) => {
                this.ingredientsPageLength = data;
            });
        },
    created () {
        fetch(`${global.ApiDomainName}/api/IngredientView/GetIngredients?ingredientName=${this.$store.state.ingredientsList[0].ingredientName}&storeId=${this.$store.state.ingredientsList[0].storeId}`)
        .then((response) => {
            return response.json()
        })
        .then((data) => {
            for(let index in data)
            {
                this.ingredients.push(data[index])
            }

        })
    }
    }
}
</script>