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
       <div v-for="(ingredient, index) in ingredients" :key=ingredient.ingredientName>
                <h3><a @click="openMap">{{ingredient.ingredientName}} from {{storeViewData.storeName}}</a></h3>
                <p>Posted On: {{ingredient.postTimeDate}}</p>    
                
                <v-btn @click="currentUpvoteStatuses[index] ? undoUpvote(ingredient.uploadId, index) : upvote(ingredient.uploadId, index);
                 currentDownvoteStatuses[index] ? undoDownvote(ingredient.uploadId, index) : null">Upvotes: {{ingredient.upvote}}</v-btn>
                
                <v-btn @click="currentDownvoteStatuses[index] ? undoDownvote(ingredient.uploadId, index) : downvote(ingredient.uploadId, index);
                 currentUpvoteStatuses[index] ? undoUpvote(ingredient.uploadId, index) : null">Downvotes: {{ingredient.downvote}}</v-btn>
               <img :src="ingredient.photo" height="350px" width="350px" />                              
                <p>Price: ${{ingredient.price}}</p>
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
            currentUpvoteStatuses: [],
            currentDownvoteStatuses:[],

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
        },
    },
    created () {
        // Fetch the list of uploads to load at the start of the page
         fetch(`${global.ApiDomainName}/api/IngredientView/GetIngredients?ingredientName=${this.$store.state.ingredientsList[0].ingredientName}&storeId=${this.$store.state.ingredientsList[0].storeId}&pagination=${this.initialPage}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`)
            .then((response) => {
                global.ErrorHandler(this.$router,response);
                return response.json();
            })
            .then((data) => {
                data.forEach((i) => {
                    this.ingredients.push(i);
                    this.currentUpvoteStatuses.push(false);
                    this.currentDownvoteStatuses.push(false);
                });
            });
            //Fetch the pagination size of this view. 
             fetch(`${global.ApiDomainName}/api/IngredientView/GetIngredientViewPaginationSize?ingredientName=${this.$store.state.ingredientsList[0].ingredientName}&storeId=${this.$store.state.ingredientsList[0].storeId}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`)
            .then((response) => {
                global.ErrorHandler(this.$router,response);
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
        //Increments the upvote value for an upload
        upvote: function(uploadId, index) {
            if(this.$store.state.userData.userType === "Anonymous") {
                alert("You must  be logged in to upvote");
            }
            else {
            fetch(`${global.ApiDomainName}/api/IngredientView/Upvote?uploadId=${uploadId}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`,
            {
                method:"POST",
                mode:"cors"
            })
            .then((response) =>
            {
                global.ErrorHandler(this.$router,response);
                return response.json()
            })
            .then((data) => {
                if(data === false){
                    this.$router.push('/ErrorView');
                }
            });
            this.currentUpvoteStatuses[index] = true;
           }
        },
       //Increments the downvote value for an upload
        downvote: function(uploadId, index) {
             if(this.$store.state.userData.userType === "Anonymous") {
                alert("You must  be logged in to downvote");
            }
            else{
             fetch(`${global.ApiDomainName}/api/IngredientView/Downvote?uploadId=${uploadId}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`,
            {
                method:"POST",
                mode:"cors"
            })
            .then((response) =>
            {
                global.ErrorHandler(this.$router,response);
                return response.json()
            })
            .then((data) => {
                if(data === false){
                    this.$router.push('/ErrorView');
                }
            });
            this.currentDownvoteStatuses[index] = true;
            }
        },
        //Adds a -1 to the upload value of an upload to undo an previous upvote. 
        undoUpvote: function(uploadId, index) {
             if(this.$store.state.userData.userType === "Anonymous") {
                alert("You must  be logged in to upvote");
            }
            else{
            fetch(`${global.ApiDomainName}/api/IngredientView/UndoUpvote?uploadId=${uploadId}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`,
            {
                method:"POST",
                mode:"cors"
            })
            .then((response) =>
            {
                global.ErrorHandler(this.$router,response);
                return response.json()
            })
            .then((data) => {
                if(data === false){
                    this.$router.push('/ErrorView');
                }
            });
            this.currentUpvoteStatuses[index] = false;
            }
       },
       // Adds -1 to the downvote value of an upload to undo a previous downvote. 
       undoDownvote: function(uploadId) {
            if(this.$store.state.userData.userType === "Anonymous") {
                alert("You must  be logged in to downvote");
            }
            else{
             fetch(`${global.ApiDomainName}/api/IngredientView/UndoDownvote?uploadId=${uploadId}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`,
            {
                method:"POST",
                mode:"cors"
            })
            .then((response) =>
            {
                global.ErrorHandler(this.$router,response);
                 return response.json()
            })
            .then((data) => {
                if(data === false){
                    this.$router.push('/ErrorView');
                }
            });
            this.currentDownvoteStatuses = false;
            }
        },
        checkSortType: function(newSortType) {
            if(newSortType === this.sortType) {
                this.sortDirection =
                this.sortDirection === 'asc' ? 'desc' : 'asc';
            }
            this.sortType = newSortType;
        },
        //Fetch the uploads for needs to display on this view
        getIngredientViewUploads: function(page) {
            fetch(`${global.ApiDomainName}/api/IngredientView/GetIngredients?ingredientName=${this.$store.state.ingredientsList[0].ingredientName}&storeId=${this.$store.state.ingredientsList[0].storeId}&pagination=${page}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`)
            .then((response) => {
                global.ErrorHandler(this.$router,response);
                return response.json();
            })
            .then((data) => {
                data.forEach((i) => {
                    this.ingredients.push(i);
                });
            });
            // Fetch the pagination size for this view.  
             fetch(`${global.ApiDomainName}/api/IngredientView/GetIngredientViewPaginationSize?ingredientName=${this.$store.state.ingredientsList[0].ingredientName}&storeId=${this.$store.state.ingredientsList[0].storeId}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`)
            .then((response) => {
                global.ErrorHandler(this.$router,response);
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