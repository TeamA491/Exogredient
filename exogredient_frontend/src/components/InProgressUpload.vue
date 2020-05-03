<template>
  <div>
    <button @click="DeleteInProgressUpload(upload.uploadId)">X</button>
    <a>{{ upload.photo }} for {{ upload.ingredientName }}</a>
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

      fetch(`${global.ApiDomainName}/api/DeleteUpload/${this.$store.state.userData.username}/${id}?ipAddress=${this.$store.state.userData.ipAddress}`
      )
      .then((response) => {
        // Display error view based on response status code
        global.ErrorHandler(this.$router, response);
        return response.json();
      }).then((data)=> {
        console.log(data);
        // Maybe let them know if not successful
      });

      // remove this upload from the in progress array.
      this.$parent.inProgressList.splice(this.Index, 1);
    },
  },
};
</script>

<style scoped></style>
