<template>
  <div>
    <nav class="textbox">
      <router-link :to="{ name: 'upload' }">Upload</router-link>
      <router-link :to="{ name: 'home' }">Exogredient</router-link>
      <router-link :to="{ name: 'profile' }">Profile</router-link>
    </nav>
  </div>
</template>

<script>
export default {}
</script>

<style scoped>
.textbox {
  display: flex;
  justify-content: space-between;
}
</style>
