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
      // http DELETE on the in progress upload
      fetch(
        `${global.ApiDomainName}/api/UserProfile/Upload/${this.$store.getters.username}/${id}`,
        { method: "DELETE" }
      ).then((response) => {
        // Display error view based on response status code
        global.ErrorHandler(this.$router, response);
        return response.json();
      });

      // remove this upload from the in progress array.
      this.$parent.inProgressList.splice(this.Index, 1);
    },
  },
};
</script>

<style scoped></style>
