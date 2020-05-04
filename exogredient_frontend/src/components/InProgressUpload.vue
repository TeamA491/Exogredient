<template>
  <div>
    <button @click="DeleteInProgressUpload(upload.uploadId)">X</button>
    <a @click="ContinueUpload(upload.uploadId)">{{ upload.photo }} for {{ upload.ingredientName }}</a>
  </div>
</template>

<script>
import * as global from "../globalExports.js";

export default {
  props: {
    upload: {
      type: Object,
      default: {},
    },
    index: {
      type: Number,
      default: -1,
    },
  },
  methods: {
    DeleteInProgressUpload(id) {
      var formD = new FormData();
      formD.append(global.UniqueIDKey, id);
      formD.append(global.UsernameKey, this.$store.state.userData.username);
      formD.append(global.IPAddressKey, this.$store.state.userData.ipAddress);

      fetch(`${global.ApiDomainName}/api/DeleteUpload`, {
        method: "POST",
        mode: "cors",
        body: formD
      }).then((response) => {
        // Display error view based on response status code
        global.ErrorHandler(this.$router, response);

        return response.json();
      }).then((data)=> {
        console.log(data);
      });

      // remove this upload from recent upload array.
      this.$parent.inProgressList.splice(this.Index, 1);
    },
    ContinueUpload(id) {
      var formD = new FormData();
      formD.append(global.UniqueIDKey, id);
      formD.append(global.UsernameKey, this.$store.state.userData.username);
      formD.append(global.IPAddressKey, this.$store.state.userData.ipAddress);

      fetch(`${global.ApiDomainName}/api/ContinueUpload`, {
        method: "POST",
        mode: "cors",
        body: formD
      }).then((response) => {
        // Display error view based on response status code
        global.ErrorHandler(this.$router, response);

        return response.json();
      }).then((data)=> {
        console.log(data);
        var image = new File([data[global.ImageResponseKey]], data[global.ImageNameResponseKey]);

        this.$store.dispatch("updateInProgressUploadName", data[global.IngredientNameResponseKey]);
        this.$store.dispatch("updateInProgressUploadDescription", data[global.DescriptionResponseKey]);
        this.$store.dispatch("updateInProgressUploadPrice", data[global.PriceResponseKey]);
        this.$store.dispatch("updateInProgressUploadPriceUnit", data[global.PriceUnitResponseKey]);
        this.$store.dispatch("updateInProgressUploadRating", data[global.RatingResponseKey]);
        this.$store.dispatch("updateInProgressUploadImage", image);
        this.$store.dispatch("updateInProgressUploadId", id);
        this.$router.push("/upload");
      });

      // remove this upload from recent upload array.
      this.$parent.inProgressList.splice(this.Index, 1);
    },
  },
};
</script>

<style scoped></style>
