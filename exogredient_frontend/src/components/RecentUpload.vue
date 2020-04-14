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
        `https://localhost:44354/api/UserProfile/Upload/${this.$store.getters.username}/${id}`,
        { method: "DELETE" }
      );

      // remove this upload from recent upload array.
      this.$parent.recentUploadList.splice(this.Index, 1);
    }
  }
};
</script>

<style lang="scss" scoped></style>
