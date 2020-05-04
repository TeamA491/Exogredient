<template>
  <div>
    <button @click="DeleteRecentUpload(upload.uploadId)">X</button>
    <b
      >{{ upload.ingredientName }} {{ upload.postTimeDate }} | U:
      {{ upload.upvotes }} D: {{ upload.downvote }}</b
    >
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
    DeleteRecentUpload(id) {
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
      this.$parent.recentUploadList.splice(this.Index, 1);
    },
  },
};
</script>

<style lang="scss" scoped></style>
