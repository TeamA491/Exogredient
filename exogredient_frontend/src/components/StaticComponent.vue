<template>
  <div class="static-view">
    <nav>
      <button onclick="SendRegistration()">Upload</button>
      <button onclick="al()">Profile</button>
    </nav>
    <!-- <div>
            <button onclick="GetUploadView()">Upload</button> 
            <button onclick="al()">Profile</button>
    </div>-->

    <h1>exogredient</h1>

    <div>
      <input type="text" placeholder="Ingredient Search..." name="search" v-model="ingredientName" />
      <input
        ref="autocomplete"
        type="text"
        placeholder="Street Address..."
        name="Street"
        v-model="address"
      />
      <input type="text" placeholder="Mile Range..." name="Mile" v-model="radius" />
      <button @click="search()">Search</button>
    </div>

    <div>
      <button>Ingredient</button>
      <button>Store</button>
    </div>

    <div id="demo">___________________________</div>
  </div>
</template>

<script>
import state from "../state.js";
import StoreViews from "../pages/StoresView";
export default {
  name: "staticComponents",
  mounted() {
    this.autocomplete = new google.maps.places.Autocomplete(
      this.$refs.autocomplete,
      ["geometry"]
    );
    this.autocomplete.setFields(["geometry"]);
    this.autocomplete.addListener("place_changed", () => {
      let place = this.autocomplete.getPlace();
      console.log(place.geometry.location.lat());
      console.log(place.geometry.location.lng());
    });
  },
  data() {
    return {
      ingredientName: "",
      latitude: null,
      longitude: null,
      radius: null,
      pagination: null,
      searchFailureCount: 1382,
      username: "",
      ipAddress: "",
      address: ""
    };
  },
  methods: {
    search: async function() {},
    getGeocode: async function() {
      // let response = await fetch(`https://maps.googleapis.com/maps/api/geocode/json?address=1600+Amphitheatre+Parkway,+Mountain+View,+CA&key=AIzaSyCFikYHwQacgDeO8YcfZjCYN2cyrY62UTQ`);
      // let data = await response.json();
      // this.$data.latitude = data.results[0].geometry.location.lat;
      // this.$data.longitude = data.results[0].geometry.location.lng;
      // console.log(this.$data.latitude, this.$data.longitude);
      var searchInputElement = document.getElementById("autocomplete");
      var autocomplete = new google.maps.places.Autocomplete(
        searchInputElement
      );
      autocomplete.getPlace();
    }
  }
};
</script>>