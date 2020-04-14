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
import * as global from '../globalExports.js';

export default {
  props: {
    upload: {
      type: Object,
      default: {}
    },
    index: {
      type: Number,
      default: -1
    },
  },
  methods: {
    DeleteRecentUpload(id) {
      // http DELETE on the in progress upload
      fetch(
        `${global.ApiDomainName}/api/UserProfile/Upload/${this.$store.getters.username}/${id}`,
        { method: "DELETE" }
      );

      // remove this upload from recent upload array.
      this.$parent.recentUploadList.splice(this.Index, 1);
    }
  }
};
</script>

<style lang="scss" scoped></style>
