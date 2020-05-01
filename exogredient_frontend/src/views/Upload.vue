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
      file: [],
    };
  },
  methods: {
    SubmitUpload: function (){
      // Collection information in above form
      // Convert it o json 
      // fetch post request 

      // ${44383}
      fetch(`https://localhost:44383/api/Upload/Vision`, {
        method: "POST",
        mode: "cors",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
        },

        body: JSON.stringify({
          Username: this.$store.state.username,
          File: this.file,
          IPAddress: this.$store.state.IPAddress,
        })
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
      this.file = event.target.files[0]
      console.log(this.file)
    }

  },
};
</script>

<style scoped></style>
