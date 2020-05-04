<template>
  <v-app id='app'>

    <div v-if="this.$store.state.userData.userType !== 'Anonymous'">
      <span id="username">Username: {{this.$store.state.userData.username}}</span>
      <span id="logout"><a @click="logout">Log out</a></span>
    </div>

    <nav id="hugTop" class="navbar" style="background-color: #8EE4AF;" role="navigation" aria-label="main navigation" v-if="!(this.$router.currentRoute.name === 'resetPassword')">
      <!-- Title -->
      <div class="navbar-brand">
        <a class="navbar-item" @click="goToHomePage">
          <h1 id="pageTitle"><a @click="goToHomePage">ExoGredient</a></h1>
        </a>

        <a role="button" class="navbar-burger burger" aria-label="menu" aria-expanded="false" data-target="menu" @click="showMenu"> 
          <span aria-hidden="true"></span>
          <span aria-hidden="true"></span>
          <span aria-hidden="true"></span>
        </a>
      </div>


      <div id="menu" class="navbar-menu" @click="closeMenu">

        <div class="navbar-start">
          <router-link class="navbar-item" v-if="this.$store.state.userData.userType !== 'Anonymous'" to="upload">Upload</router-link>
          <router-link class="navbar-item" v-if="this.$store.state.userData.userType !== 'Anonymous'" to="profile">Profile</router-link>
          <router-link class="navbar-item" v-if="this.$store.state.userData.userType === 'Anonymous'" to="useranalysis">Analysis</router-link>
          <router-link class="navbar-item" v-if="this.$store.state.userData.userType === 'Anonymous'" to="login">Login</router-link>
          <a class="navbar-item" v-if="this.$store.state.userData.userType === 'Anonymous'" @click="goToRegistration">Register</a>
        </div>


        <div class="navbar-end">
          <div class="navbar-item">
            <div v-if="this.$store.state.userData.userType !== 'Anonymous'">
              <span id="username">Username: {{this.$store.state.userData.username}}</span>

              <div class="buttons">
                <a class="button is-primary">
                  <span id="logout"><a @click="logout">Log out</a></span>
                </a>
              </div>
            </div>
          </div>
        </div>

      </div>

    </nav>


    <div id='static'>

      <div v-if="show">
        <div>
          <input id="address" style="border-style:solid" maxlength="100" ref="autocomplete" type="text" placeholder="Street Address..." name="street" v-model="searchData.address">
          <br/>
          <input id="searchTerm" style="border-style:solid" type="text" maxlength="100" :placeholder="searchPlaceholder" name="search" v-model="searchData.searchTerm">
          <input id="radius" style="border-style:solid" type="text" placeholder="Mile Range..." name="mile" v-model="searchData.radius">
          <button id="searchButton" class="button is-light" @click="search">Search</button>
        </div>
        <div>
          <input type="radio" name="searchBy" value="ingredient" id="ingredient" @click="setToIngredientSearch">
          <label for="ingredient"> Ingredient </label>
          <input type="radio" name="searchBy" value="store" id="store" @click="setToStoreSearch">
          <label for="store"> Store </label>
        </div>
        <span>Search by: <strong>{{searchData.searchBy}}</strong></span>
      </div>
      
      <div id="demo">___________________________</div>
    </div>
    <router-view></router-view>
  </v-app>
</template>

<script>
  import * as global from "./globalExports.js";
  export default {
    name: "App",
    watch:{
      $route (to, from){
        this.$store.dispatch('updateRouteTo',to.name);
        this.$store.dispatch('updateRouteFrom',from.name);
        if(to.name !== global.HomePage){
          this.show = false;
        }else{
          this.show = true;
          location.reload();
        }
      }
    },
    async mounted(){
      // Filter unknown URL
      let routeExist = false;
      let currentPath = this.$router.currentRoute.path;

      for(let route of this.$router.options.routes){
        routeExist = routeExist || (route.path === currentPath)
      }
      if(!routeExist){
        this.$router.push('/pageNotFound');
      }

      // Ask for Geolocation permission
      if(navigator.geolocation){
        console.log("in if");
        navigator.geolocation.getCurrentPosition(this.savePosition);
      }else {
        alert("Geolocation is not supported by this browser.");
      }

      var ipAddressResponse = await fetch("https://ipapi.co/json");
      var ipAddressJson = await ipAddressResponse.json();
      this.$store.dispatch('updateIpAddress',ipAddressJson.ip);
      this.$store.dispatch('updateLocation',ipAddressJson.region);
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
          radius: "",
          searchBy: "ingredient",
          address: ''
        },
        searchPlaceholder: "Ingredient Search...",
        isPlaceSelected: false,
        isGeoLocationAllowed: false,
        show: true
      }
    },
    methods:{
      savePosition: function(position){
        this.$data.searchData.lat = position.coords.latitude;
        this.$data.searchData.lng = position.coords.longitude;
        this.$data.isGeoLocationAllowed = true;
      },
      closeMenu: function(){
        document.querySelector(".navbar-menu").classList.remove('is-active');
      },
      showMenu: function(){
        document.querySelector(".navbar-menu").classList.toggle('is-active');
      },
      logout: function(){
        this.$store.dispatch('updateUsername', 'Anonymous');
        this.$store.dispatch('updateUserType', 'Anonymous');
        this.$store.dispatch('updateToken', '');
        this.goToHomePage();
      },
      goToHomePage: function(){
        if(this.$router.currentRoute.path !== '/'){
          this.$router.push('/');
          location.reload();
        }else{
          location.reload();
        }
      },
      goToRegistration: function(){
        if(this.$store.state.userData.location === "California"){
          if(this.$router.currentRoute.name !== 'register'){
            this.$router.push('/register');
          }else{
            location.reload();
          }
        }else{
          alert("You must be in California to register!");
        }
      },
      search: async function(){
        // Check if an address was selected from autocompletes.
        if(!this.isGeoLocationAllowed && !this.$data.isPlaceSelected){
          alert("Address must be selected from the autocompletes or allow geolocation for this website.");
          return;
        }

        // Check if search term is empty.
        if(this.$data.searchData.searchTerm.length < global.MinimumSearchTermLength){
          alert(`The ${this.searchData.searchBy} name cannot be empty`);
          return;
        }

        // Give default radius if it is empty
        if(this.searchData.radius !== ""){
          // Check if the radius a number between 1 and 100.
          if( (isNaN(parseFloat(this.$data.searchData.radius)) || 
              !isFinite(this.$data.searchData.radius))
              || 
              (this.$data.searchData.radius < global.MinimumRadius || 
              this.$data.searchData.radius > global.MaximumRadius)){
            alert("Radius must be a number between 1-100");
            return;
          }
        }

        // Update searchData in Vuex.
        this.$store.dispatch('updateSearchData', this.$data.searchData);

        // Fetch the number of total results for the search from backend.
        let paginationResponse = 
          await fetch(`${global.ApiDomainName}/api/getTotalNum?`
          + `searchTerm=${this.searchData.searchTerm}&latitude=${this.searchData.lat}`
          + `&longitude=${this.searchData.lng}`
          + `&radius=${this.searchData.radius}`
          + `&searchBy=${this.searchData.searchBy}&username=${this.$store.state.userData.username}`
          + `&ipAddress=${this.$store.state.userData.ipAddress}`);

        // Handle any error from the HTTP status code.
        global.ErrorHandler(this.$router,paginationResponse);

        // Get the total results number returned from backend.
        let totalResultsNum = await paginationResponse.json();
        this.$store.dispatch('updateStoreResultsTotalNum',totalResultsNum);

        // Check if the total results number is 0.
        if(totalResultsNum === 0){
          if(this.$router.currentRoute.path !== "/SearchResultsView"){
            this.$router.push("/search");
          }
          return;
        }
        
        // Fetch search results for the first page from backend. 
        let resultsResponse = 
            await fetch(`${global.ApiDomainName}/api/getStoreResults?`
            + `searchTerm=${this.searchData.searchTerm}&latitude=${this.searchData.lat}`
            + `&longitude=${this.searchData.lng}&radius=${this.searchData.radius}`
            + `&searchBy=${this.searchData.searchBy}&lastStoreData=${global.NoLastStoreData}`
            + `&lastStoreId=${global.NoLastStoreId}&skipPages=${global.DefaultSkipPages}`
            + `&sortOption=${global.DefaultSortOption}&fromSmallest=${global.DefaultFromSmallest}`
            + `&username=${this.$store.state.userData.username}&ipAddress=${this.$store.state.userData.ipAddress}`);

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
          this.$router.push("/search");
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
  margin-top: 10vh;
}
#app{
  padding:5% 10% 10% 10%;
}
#address{
  width:100%;
}
#searchTerm{
  width:38%;
  margin: 1%;
}
#radius{
  width:38%;
  margin: 1%;
}
#searchButton{
  width:20%
}
#username{
  padding-right: 2%;
}
#logout{
  padding-left: 2%;
}
#pageTitle{
  font-size: 1.5em;
  font-weight: bold;
}
#hugTop{
  position: fixed; /* fixing the position takes it out of html flow - knows
                   nothing about where to locate itself except by browser
                   coordinates */
  left:0;           /* top left corner should start at leftmost spot */
  top:0;            /* top left corner should start at topmost spot */
  width:100vw;      /* take up the full browser width */
  z-index:200;  /* high z index so other content scrolls underneath */
  height: 10vh;     /* define height for content */
}
</style>
