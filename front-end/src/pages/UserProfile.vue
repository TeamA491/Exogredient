<template>
  <div>
    <h1 class="center">{{ username }}: {{ score }}</h1>

    <!-- display business owners functionality  -->
    <div class="center">
      <h3>Owned Store's</h3>
    </div>

    <!-- display the recent uploads  -->
    <div>
      <button @click="GetRecentUploads">
        {{ recentUploadStatus ? "Hide Recent Uploads" : "Show Recent Uploads" }}
      </button>
      <div v-if="recentUploadStatus" class="content">
        <div v-for="(item, index) in recentUploadList" :key="index">
          <RecentUpload :upload="item" :index="index"> </RecentUpload>
        </div>
      </div>
    </div>

    <!-- display the inprogress uploads  -->
    <div class="inProgress">
      <Button @click="GetInProgressUploads">{{
        inProgressStatus
          ? "Hide In Progress Uploads"
          : "Show In Progress Uploads"
      }}</Button>
      <div v-if="inProgressStatus" class="content">
        <div v-for="(item, index) in inProgressList" :key="index">
          <InProgressUpload
            :upload="item"
            :index:="index"
          ></InProgressUpload>
        </div>
      </div>
    </div>

    <!-- display the save list  -->
    <div class="saveList">
      <button @click="GetSaveList">
        {{ saveListStatus ? "Hide SaveList" : "Show SaveList" }}
      </button>
      <div v-if="saveListStatus" Class="content">
        <div v-for="(item, index) in saveList" :key="index">
          <SaveList :saveItem="item" :index="index"></SaveList>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import SaveList from "@/components/SaveList.vue";
import InProgressUpload from "@/components/InProgressUpload.vue";
import RecentUpload from "@/components/RecentUpload.vue";

export default {
  components: {
    SaveList,
    InProgressUpload,
    RecentUpload,
  },
  data() {
    return {
      username: String,
      score: 0,
      saveList: [],
      saveListStatus: false,
      saveListPage: 1,
      inProgressList: [],
      inProgressStatus: false,
      recentUploadList: [],
      recentUploadStatus: false,
    };
  },
  methods: {
    GetRecentUploads() {
      if (!this.recentUploadStatus) {
        // Change the show recent uploads button to hide
        this.recentUploadStatus = true;

        // Fetch the RecentUploads
        fetch(
          `https://localhost:44354/api/Userprofile/Recentuploads/${this.$store.state.username}/0`
        )
          .then(response => response.json())
          .then(data => {
            data.forEach(i => {
              this.recentUploadList.push(i);
            });
          });
      } else {
        this.recentUploadStatus = false;
        this.recentUploadList = [];
      }
    },

    GetInProgressUploads() {
      if (!this.inProgressStatus) {
        // Change the show InProgress uploads button to hide
        this.inProgressStatus = true;

        // Fetch the InProgress uploads
        fetch(
          `https://localhost:44354/api/UserProfile/InProgressUploads/${this.$store.state.username}/0`
        )
          .then(response => response.json())
          .then(data => {
            data.forEach(i => {
              this.inProgressList.push(i);
            });
          });
      } else {
        this.inProgressStatus = false;
        this.inProgressList = [];
      }
    },

    GetSaveList() {
      if (!this.saveListStatus) {
        // Change the show saveList button to hide
        this.saveListStatus = true;

        // Fetch the save lists for a user
        fetch(
          `https://localhost:44354/api/UserProfile/SaveList/${this.$store.state.username}/0`
        )
          .then(response => response.json())
          .then(data => {
            // Add the first page of the savelist to the saveList data object
            data.forEach(i => {
              this.saveList.push(i);
            });
          });
      } else {
        this.saveListStatus = false;
        this.saveList = [];
      }
    },
  },
  created() {
    // Retrieve user name from the vuex store.
    this.username = this.$store.getters.username;

    // Make an API call to calculate the score for this user \.
    fetch(
      `https://localhost:44354/api/UserProfile/ProfileScore/${this.$store.state.username}`
    )
      .then(response => response.json())
      .then(data => {
        // Loop through the array and sum
        let upvotes = 0;
        let downvotes = 0;
        data.forEach((element, index) => {
          upvotes += element.uploadUpvote;
          downvotes += element.uploadDownvote;
        });
        this.score = upvotes - downvotes;
      });
  },
};
</script>

<style scoped>
.center {
  text-align: center;
}
.content {
  height: 200px;
  width: 350px;
  overflow: auto;
}
.saveList {
  position: fixed;
  bottom: 100;
  right: 0;
}
.inProgress {
  position: fixed;
  top: 0;
  right: 0;
}
</style>
