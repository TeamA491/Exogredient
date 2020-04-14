<template>
  <v-app id="app">
    <div id="static">
      <nav>
        <v-btn to="upload">Upload</v-btn>
        <v-btn to="profile">Profile</v-btn>
        <!-- <router-link :to="{ name: 'upload' }" tag="button">Upload</router-link>
        <router-link :to="{ name: 'profile' }" tag="button"
          >Profile</router-link -->
        
      </nav>
      <h1><router-link :to="{ name: 'home' }"> exogredient </router-link></h1>
      <div>
        <input
          style="border-style:solid"
          size="65"
          ref="autocomplete"
          type="text"
          placeholder="Street Address..."
          name="Street"
          v-model="address"
        />
        <br />
        <input
          style="border-style:solid"
          type="text"
          :placeholder="searchPlaceholder"
          name="search"
          v-model="searchTerm"
        />
        <input 
          style="border-style:solid"
          type="text"
          placeholder="Mile Range..."
          name="Mile"
          v-model="radius"
        />
        <v-btn @click="search">Search</v-btn>
        <!-- <router-link to = '/StoresView'> 
            
          </router-link> -->
      </div>

      <div>
        <button class="button is-light are-small" @click="setToIngredientSearch">Ingredient</button>
        <button class="button is-light are-small" @click="setToStoreSearch">Store</button>
      </div>
      <span
        >Search by: <strong>{{ searchBy.slice(2) }}</strong></span
      >
      <div id="demo">___________________________</div>
    </div>
    <router-view></router-view>
  </v-app>
</template>

<script>
export default {
  name: "App",
  mounted() {
    this.autocomplete = new google.maps.places.Autocomplete(
      this.$refs.autocomplete
    );
    this.autocomplete.setFields(["geometry.location", "formatted_address"]);
    this.autocomplete.addListener("place_changed", () => {
      let place = this.autocomplete.getPlace();
      this.lat = place.geometry.location.lat();
      this.lng = place.geometry.location.lng();
      this.address = place.formatted_address;
      // console.log(place.formatted_address);
      console.log(place.geometry.location.lat());
      console.log(place.geometry.location.lng());
    });
  },
  data() {
    return {
      searchTerm: "",
      lat: null,
      lng: null,
      radius: null,
      pagination: 1,
      failureCount: 0,
      username: "anonymous",
      ipAddress: "127.1.1.0",
      address: "",
      searchPlaceholder: "Ingredient Search...",
      searchBy: "byIngredient"
    };
  },
  methods: {
    search: async function() {
      if (this.searchBy === "byIngredient") {
        this.$store.dispatch("updateIngredientName", this.searchTerm);
      } else {
        this.$store.dispatch("updateIngredientName", "");
      }
      let response = await fetch(
        `https://localhost:5001/api/search/${this.searchBy}?` +
          `searchTerm=${this.searchTerm}&latitude=${this.lat}&longitude=${this.lng}&radius=${this.radius}` +
          `&pagination=${this.pagination}&failureCount=${this.failureCount}&username=${this.username}&ipAddress=${this.ipAddress}`
      );
      let store = await response.json();
      console.log(store);
      this.$store.dispatch("updateStoreResults", store);
      if (this.$router.currentRoute.path !== "/StoresView") {
        this.$router.push("/StoresView");
      }
    },
    setToIngredientSearch: function() {
      this.searchPlaceholder = "Ingredient Search...";
      this.searchBy = "byIngredient";
    },
    setToStoreSearch: function() {
      this.searchPlaceholder = "Store Search...";
      this.searchBy = "byStore";
    }
  }
};
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
