<template>
  <div id='app'>
    <div id='static'>
      <nav>
        <button onclick="SendRegistration()">Upload</button> 
        <button onclick="al()">Profile</button>
      </nav>
      <h1>exogredient</h1>
      <div>
        <input size="65" ref="autocomplete" type="text" placeholder="Street Address..." name="Street" v-model="address">
        <br/>
        <input type="text" :placeholder="searchPlaceholder" name="search" v-model="searchTerm">
        <input type="text" placeholder="Mile Range..." name="Mile" v-model="radius">
        <button @click="search">Search</button>
        <!-- <router-link to = '/StoresView'> 
          
        </router-link> -->
      </div>

      <div>
        <button @click="setToIngredientSearch">Ingredient</button>
        <button @click="setToStoreSearch">Store</button>
      </div>
      <span>Search by: <strong>{{searchBy.slice(2)}}</strong></span>
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
            this.lat = place.geometry.location.lat();
            this.lng = place.geometry.location.lng();
            this.address = place.formatted_address;
            // console.log(place.formatted_address);
            console.log(place.geometry.location.lat());
            console.log(place.geometry.location.lng());
        });
    },
    data(){
        return{
            searchTerm: '',
            lat: null,
            lng: null,
            radius: null,
            pagination: 1,
            failureCount: 0,
            username: 'testuser',
            ipAddress: '127.1.1.0',
            address: '',
            searchPlaceholder: "Ingredient Search...",
            searchBy: "byIngredient"
        }
    },
    methods:{
        search: async function(){
          if(this.searchBy === "byIngredient"){
            this.$store.dispatch('updateIngredientName',this.searchTerm);
          }else{
            this.$store.dispatch('updateIngredientName',"");
          }
          let response = 
          await fetch(`https://localhost:5001/api/search/${this.searchBy}?`
          + `searchTerm=${this.searchTerm}&latitude=${this.lat}&longitude=${this.lng}&radius=${this.radius}`
          + `&pagination=${this.pagination}&failureCount=${this.failureCount}&username=${this.username}&ipAddress=${this.ipAddress}`);
          let store = await response.json();
          console.log(store);
          this.$store.dispatch('updateStoreResults',store);
          if(this.$router.currentRoute.path !== "/StoresView"){
            this.$router.push("/StoresView");
          }
        },
        setToIngredientSearch: function(){
          this.searchPlaceholder = "Ingredient Search...";
          this.searchBy = "byIngredient";
        },
        setToStoreSearch: function(){
          this.searchPlaceholder = "Store Search...";
          this.searchBy = "byStore";
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
