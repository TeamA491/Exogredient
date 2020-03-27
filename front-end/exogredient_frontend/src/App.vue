<template>
  <div id='app'>
    <div id='static'>
      <nav>
        <button onclick="SendRegistration()">Upload</button> 
        <button onclick="al()">Profile</button>
      </nav>
      <h1>exogredient</h1>
      <div>
        <input size="65" maxlength="100" ref="autocomplete" type="text" placeholder="Street Address..." name="street" v-model="searchData.address">
        <br/>
        <input type="text" maxlength="100" :placeholder="searchPlaceholder" name="search" v-model="searchData.searchTerm">
        <input type="text" placeholder="Mile Range..." name="mile" v-model="searchData.radius">
        <button @click="search">Search</button>
        <!-- <router-link to = '/StoresView'> 
          
        </router-link> -->
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
            // console.log(place.formatted_address);
            console.log(place.geometry.location.lat());
            console.log(place.geometry.location.lng());
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
          failureCount: 0,
          searchPlaceholder: "Ingredient Search...",
            
        }
    },
    methods:{
        search: async function(){
          if( (isNaN(parseFloat(this.$data.searchData.radius)) || !isFinite(this.$data.searchData.radius)) || 
              (this.$data.searchData.radius < 1 || this.$data.searchData.radius > 100)){
            alert("Radius must be a number between 1-100");
            return;
          }
          this.$store.dispatch('updateSearchData', this.$data.searchData);
          let paginationResponse = 
          await fetch(`https://localhost:5001/api/search/getTotalNum?`
          + `searchTerm=${this.searchData.searchTerm}&latitude=${this.searchData.lat}&longitude=${this.searchData.lng}&radius=${this.searchData.radius}`
          + `&searchBy=${this.searchData.searchBy}&failureCount=${this.failureCount}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`);
          let totalResultsNum = await paginationResponse.json();
          this.$store.dispatch('updateTotalResultsNum',totalResultsNum);
          console.log(totalResultsNum);
          
          let resultsResponse = 
          await fetch(`https://localhost:5001/api/search/getResults?`
          + `searchTerm=${this.searchData.searchTerm}&latitude=${this.searchData.lat}&longitude=${this.searchData.lng}&radius=${this.searchData.radius}`
          + `&searchBy=${this.searchData.searchBy}&lastStoreData=${-1}&lastStoreId=${0}&sortOption=${"distance"}&fromSmallest=${true}`
          + `&failureCount=${this.failureCount}&username=${this.$store.state.username}&ipAddress=${this.$store.state.ipAddress}`);
          let stores = await resultsResponse.json();
          console.log(stores);

          this.$store.dispatch('updateStoreResults',stores);
          //this.$store.dispatch('updateAddress', this.$data.address);
          if(this.$router.currentRoute.path !== "/SearchResultsView"){
            this.$router.push("/SearchResultsView");
          }else{
            console.log("in search results view")
            this.$store.dispatch('updateSortOption', {by:'distance', fromSmallest: true});
          }
        },
        setToIngredientSearch: function(){
          this.$data.searchPlaceholder = "Ingredient Search...";
          this.$data.searchData.searchBy = "ingredient";
        },
        setToStoreSearch: function(){
          this.$data.searchPlaceholder = "Store Search...";
          this.$data.searchData.searchBy = "store";
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
