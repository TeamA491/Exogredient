<template>
    <div>
        <v-btn @click="checkSortType('upvote'); sortedIngredients()">Sort By Upvotes</v-btn>
        <v-btn @click="checkSortType('postTimeDate'); sortedIngredients()">Sort By Date</v-btn>
        <div>
            <v-pagination
             v-model="ingredientsPage"
             :dark = "true"
             :length="ingredientsPageLength"
             ></v-pagination>
        </div>
       <div v-for="ingredient in ingredients" :key=ingredient.ingredientName>
                <h3><a @click="openMap">{{ingredient.ingredientName}} from {{storeViewData.storeName}}</a></h3>
                <p>Posted On: {{ingredient.postTimeDate}}</p>    
                
                <v-btn @click="undoUpvoteStatus ? undoUpvote(ingredient.uploadId) : upvote(ingredient.uploadId);
                 undoDownvoteStatus ? undoDownvote(ingredient.uploadId) : null">Upvotes: {{ingredient.upvote}}</v-btn>
                
                <v-btn @click="undoDownvoteStatus ? undoDownvote(ingredient.uploadId) : downvote(ingredient.uploadId);
                 undoUpvoteStatus ? undoUpvote(ingredient.uploadId) : null">Downvotes: {{ingredient.downvote}}</v-btn>
               <img :src="ingredient.photo" height="550px" width="550px" />                              
                <p>Price: {{ingredient.price}}</p>
                <p>Rating:{{ingredient.rating}}</p>
                <p>Uploaded by: {{ingredient.uploader}}</p>
                <p>Description: {{ingredient.description}}</p>
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
            ingredientsPage: 1,
            initialPage: 0,

            sortType: 'postDateTime',
            sortDirection: 'asc'

        }
    },
    computed:{
        //Get information of store from previous view. 
        storeViewData: function () {
            return this.$store.state.storeViewData;
        },
        sortedIngredients: function() {
            return this.ingredients.sort((a,b) => {
            let modifier = 1;
            if(this.sortDirection ==='desc'){
                modifier = -1;
            }
            if(a[this.sortType] < b[this.sortType]){
                return -1 * modifier;
            }
             if(a[this.sortType] > b[this.sortType]){
                return modifier;
            }
            return 0;})
        }
    },
    created () {
        // Fetch the list of uploads to load at the start of the page
         fetch(`${global.ApiDomainName}/api/IngredientView/GetIngredients?ingredientName=${this.$store.state.ingredientsList[0].ingredientName}&storeId=${this.$store.state.ingredientsList[0].storeId}&pagination=${this.initialPage}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`)
            .then((response) => {
                //global.ErrorHandler(this.$router,response);
                return response.json();
            })
            .then((data) => {
                data.forEach((i) => {
                    this.ingredients.push(i);
                });
            });
            //Fetch the pagination size of this view. 
             fetch(`${global.ApiDomainName}/api/IngredientView/GetIngredientViewPaginationSize?ingredientName=${this.$store.state.ingredientsList[0].ingredientName}&storeId=${this.$store.state.ingredientsList[0].storeId}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`)
            .then((response) => {
              //  global.ErrorHandler(this.$router,response);
                return response.json();
            })
            .then((data) => {
                this.ingredientsPageLength = data;
            });
    },
    
    methods: {
        openMap: function(){
            // Opens Google Maps direction to the store from the center of search.
            var encodedOriginAddress = encodeURIComponent(this.$store.state.searchData.address);
            window.open(`https://www.google.com/maps/dir/?api=1`
                + `&origin=${encodedOriginAddress}&destination=${this.storeViewData.storeName}`
                + `&destination_place_id=${this.storeViewData.placeId}&travelmode=driving`, "_blank");
        },
        upvote: function(uploadId) {
            fetch(`${global.ApiDomainName}/api/IngredientView/Upvote?uploadId=${uploadId}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`,
            {
                method:"POST",
                mode:"cors"
            })
            .then((response) =>
            {
                return response.json()
            })
            .then((data) => {
                if(data === false){
                    this.$router.push('/ErrorView');
                }
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
                return response.json()
            })
            .then((data) => {
                if(data === false){
                    this.$router.push('/ErrorView');
                }
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
                return response.json()
            })
            .then((data) => {
                if(data === false){
                    this.$router.push('/ErrorView');
                }
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
                 return response.json()
            })
            .then((data) => {
                if(data === false){
                    this.$router.push('/ErrorView');
                }
            });
            this.undoDownvoteStatus = false;
        },
        checkSortType: function(newSortType) {
            if(newSortType === this.sortType) {
                this.sortDirection =
                this.sortDirection === 'asc' ? 'desc' : 'asc';
            }
            this.sortType = newSortType;
        },
        getIngredientViewUploads: function(page) {
            fetch(`${global.ApiDomainName}/api/IngredientView/GetIngredients?ingredientName=${this.$store.state.ingredientsList[0].ingredientName}&storeId=${this.$store.state.ingredientsList[0].storeId}&pagination=${page}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`)
            .then((response) => {
               // global.ErrorHandler(this.$router,response);
                return response.json();
            })
            .then((data) => {
                data.forEach((i) => {
                    this.ingredients.push(i);
                });
            });

             fetch(`${global.ApiDomainName}/api/IngredientView/GetIngredientViewPaginationSize?ingredientName=${this.$store.state.ingredientsList[0].ingredientName}&storeId=${this.$store.state.ingredientsList[0].storeId}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`)
            .then((response) => {
               // global.ErrorHandler(this.$router,response);
                return response.json();
            })
            .then((data) => {
                this.ingredientsPageLength = data;
            });
        }  
    },
     
    watch : {
        ingredientsPage(newValue,oldValue) {
            this.ingredients = [];
            fetch(`${global.ApiDomainName}/api/IngredientView/GetIngredients?ingredientName=${this.$store.state.ingredientsList[0].ingredientName}&storeId=${this.$store.state.ingredientsList[0].storeId}&pagination=${newValue - 1}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`)
            .then((response) => {
              //  global.ErrorHandler(this.$router, response);
                return response.json();
            })
            .then((data) => {
                data.forEach((i) => {
                    this.ingredients.push(i);
                });
            });
        }
    }
}
</script>