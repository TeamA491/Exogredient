<template>
  <div>
    <!--<div class="navbar"></div>-->
    <form>
      <input type="file" @change="ImageUpload_OnChange" accept="image/*" required /> <br />
      <input type="text" name="" id="" placeholder="category" required /> <br />
      <input type="text" name="" id="" placeholder="amount" required /> <br />
    </form>
  <input @click="SubmitUpload" type="submit" value="Submit" />

  </div>
</template>

<script>
import * as global from "../globalExports.js";

export default {
  data() {
    return {
      files: new FormData(),
    };
  },
  methods: {
    SubmitUpload: function (){
      // Collection information in above form
      // Convert it o json 
      // fetch post request 

      // ${44383}
      fetch(`https://localhost:44383/api/Vision/${this.$store.state.username}/${this.files}/${this.$store.state.ipAddress}`, {
        method: "GET",
        mode: "cors",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
        },
      })
      .then((response) =>{
        // erro handle if !response.ok
        return response.json()
      })
      .then((data)=>{
        alert(data)
        console.log(data);
        
      })
    },
    ImageUpload_OnChange(event) {
      this.files.append("photo", event.target.files[0])
      console.log(this.files)
    }

  },
};
</script>

<style scoped></style>
