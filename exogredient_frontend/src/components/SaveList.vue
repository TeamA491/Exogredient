<template>
  <div style="display: flex;">
    <button @click="DeleteSaveItem(saveItem)">X</button>
    <p>{{ saveItem.ingredientName }} @ store: {{ saveItem.storeId }}</p>
  </div>
</template>

<script>
import * as global from '../globalExports.js';

export default {
  props: {
    saveItem: {
      type: Object,
      default: {},
    },
    index: {
      type: Number,
      default: -1,
    },
  },
  methods: {
    DeleteSaveItem(saveItem) {
      // http DELETE on the save item
      fetch(
        `${global.ApiDomainName}/api/UserProfile/SaveList/${saveItem.username}/${saveItem.storeId}/${saveItem.ingredientName}`,
        { method: "DELETE" }
      );

      // remove this save list item from the parent saveList array
      this.$parent.saveList.splice(this.Index, 1);
    },
  },
};
</script>

<style scoped></style>
