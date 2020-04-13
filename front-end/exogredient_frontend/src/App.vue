<template>
  <div id='app'>
    <div id='static'>
      <nav>
        <button onclick="SendRegistration()">Upload</button> 
        <button onclick="al()">Profile</button>
      </nav>
      <h1><a @click="goToHomePage">exogredient</a></h1>
      <div>
        <input size="65" maxlength="100" ref="autocomplete" type="text" placeholder="Street Address..." name="street" v-model="searchData.address">
        <br/>
        <input type="text" maxlength="100" :placeholder="searchPlaceholder" name="search" v-model="searchData.searchTerm">
        <input type="text" placeholder="Mile Range..." name="mile" v-model="searchData.radius">
        <button @click="search">Search</button>
      </div>
      <div>
        <button @click="setToIngredientSearch">Ingredient</button>
        <button @click="setToStoreSearch">Store</button>
      </div>
      <span>Search by: <strong>{{searchData.searchBy}}</strong></span>
      <div id="demo">___________________________</div>
    </div>
    <router-view></router-view>
  </div>
</template>

<script>
  import * as global from "./globalExports.js";
  export default {
    name: "App",
    mounted(){
      this.autocomplete = new google.maps.places.Autocomplete(this.$refs.autocomplete);
      this.autocomplete.setFields(['geometry.location','formatted_address']);
      this.autocomplete.addListener('place_changed',()=>{
          let place = this.autocomplete.getPlace();
          this.$data.searchData.lat = place.geometry.location.lat();
          this.$data.searchData.lng = place.geometry.location.lng();
          this.$data.searchData.address = place.formatted_address;
          this.$data.isPlaceSelected = true;
      });
    },
    data(){
      return{
        searchData:{
          searchTerm: '',
          lat: null,
          lng: null,
          radius: null,
          searchBy: "ingredient",
          address: '',
        },
        searchPlaceholder: "Ingredient Search...",
        isPlaceSelected: false
          
      }
    },
    methods:{
      goToHomePage: function(){
        if(this.$router.currentRoute.path !== '/'){
          this.$router.push('/');
          location.reload();
        }else{
          location.reload();
        }
      },
      
      search: async function(){
        // Check if an address was selected from autocompletes.
        if(!this.$data.isPlaceSelected){
          alert("Address must be selected from the autocompletes");
          return;
        }

        // Check if search term is empty.
        if(this.$data.searchData.searchTerm.length < global.MinimumSearchTermLength){
          alert("The search term cannot be empty");
          return;
        }

        // Check if the radius a number between 1 and 100.
        if( (isNaN(parseFloat(this.$data.searchData.radius)) || 
             !isFinite(this.$data.searchData.radius))
             || 
            (this.$data.searchData.radius < global.MinimumRadius || 
            this.$data.searchData.radius > global.MaximumRadius)){
          alert("Radius must be a number between 1-100");
          return;
        }

        // Update searchData in Vuex.
        this.$store.dispatch('updateSearchData', this.$data.searchData);

        // Fetch the number of total results for the search from backend.
        let paginationResponse = 
          await fetch(`${global.ApiDomainName}/api/search/getTotalNum?`
          + `searchTerm=${this.searchData.searchTerm}&latitude=${this.searchData.lat}`
          + `&longitude=${this.searchData.lng}&radius=${this.searchData.radius}`
          + `&searchBy=${this.searchData.searchBy}&username=${this.$store.state.username}`
          + `&ipAddress=${this.$store.state.ipAddress}`);

        // Handle any error from the HTTP status code.
        global.ErrorHandler(this.$router,paginationResponse);

        // Get the total results number returned from backend.
        let totalResultsNum = await paginationResponse.json();
        this.$store.dispatch('updateStoreResultsTotalNum',totalResultsNum);

        // Check if the total results number is 0.
        if(totalResultsNum === 0){
          if(this.$router.currentRoute.path !== "/SearchResultsView"){
            this.$router.push("/SearchResultsView");
          }
          return;
        }
        
        // Fetch search results for the first page from backend. 
        let resultsResponse = 
            await fetch(`${global.ApiDomainName}/api/search/getStoreResults?`
            + `searchTerm=${this.searchData.searchTerm}&latitude=${this.searchData.lat}`
            + `&longitude=${this.searchData.lng}&radius=${this.searchData.radius}`
            + `&searchBy=${this.searchData.searchBy}&lastStoreData=${global.NoLastStoreData}`
            + `&lastStoreId=${global.NoLastStoreId}&skipPages=${global.DefaultSkipPages}`
            + `&sortOption=${global.DefaultSortOption}&fromSmallest=${global.DefaultFromSmallest}`
            + `&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`);

        // Handle any error from the HTTP status code.
        global.ErrorHandler(this.$router,resultsResponse);

        // Get the list of results for search and update Vuex.
        let stores = await resultsResponse.json();
        this.$store.dispatch('updateStoreResults',stores);

        // If search is performed on SearchResultsView.
        if(this.$router.currentRoute.path === "/SearchResultsView"){
          this.$store.dispatch('updateSortOption', {by:global.SortByDistance, fromSmallest: true});
          global.Bus.$emit("updatePagination",global.DefaultPage);
        }
        // If search is performed on any other page.
        else{
          this.$store.dispatch('updateSortOption', {by:global.SortByDistance, fromSmallest: true});
          this.$store.dispatch('updateSearchResultsViewCurrentPage', global.DefaultPage);
          this.$router.push("/SearchResultsView");
        }
      },

      setToIngredientSearch: function(){
        this.$data.searchPlaceholder = "Ingredient Search...";
        this.$data.searchData.searchBy = global.SearchByIngredient;
      },
      
      setToStoreSearch: function(){
        this.$data.searchPlaceholder = "Store Search...";
        this.$data.searchData.searchBy = global.SearchByStore;
      }
    }
  }
</script>
<style>
#static {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
