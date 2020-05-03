<template>
  <div>
    <h1 class="center">{{ username }}: {{ score }}</h1>

    <!-- display business owners functionality  -->
    <div class="center">
      <h3>Owned Store's</h3>
    </div>

    <!-- display the recent uploads  -->
    <div>
      <v-btn @click="GetRecentUploads(0)">
        {{ recentUploadStatus ? "Hide Recent Uploads" : "Show Recent Uploads" }}
      </v-btn>
      <div v-if="recentUploadStatus" class="column">
        <v-pagination
          v-model="recentUploadPage"
          :value="1"
          :dark="true"
          :length="recentUploadPageLength"
        ></v-pagination>

        <div v-for="(item, index) in recentUploadList" :key="index">
          <RecentUpload :upload="item" :index="index"> </RecentUpload>
        </div>
      </div>
    </div>

    <!-- display the inprogress uploads  -->
    <div class="inProgress">
      <v-btn @click="GetInProgressUploads(0)">{{
        inProgressStatus
          ? "Hide In Progress Uploads"
          : "Show In Progress Uploads"
      }}</v-btn>
      <div v-if="inProgressStatus" class="column">
        <v-pagination
          v-model="inProgressPage"
          :value="1"
          :dark="true"
          :length="inProgressPageLength"
        ></v-pagination>

        <div v-for="(item, index) in inProgressList" :key="index">
          <InProgressUpload :upload="item" :index:="index"></InProgressUpload>
        </div>
      </div>
    </div>

    <!-- display the save list  -->
    <div class="saveList">
      <v-btn @click="GetSaveList(0)">
        {{ saveListStatus ? "Hide SaveList" : "Show SaveList" }}
      </v-btn>
      <div v-if="saveListStatus" Class="column">
        <v-pagination
          v-model="saveListPage"
          :dark="true"
          :length="saveListPageLength"
        ></v-pagination>

        <div v-for="(item, index) in saveList" :key="index">
          <SaveList :saveItem="item" :index="index"></SaveList>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import * as global from "../globalExports.js";

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
      saveListPageLength: 1,

      inProgressList: [],
      inProgressStatus: false,
      inProgressPage: 1,
      inProgressPageLength: 1,

      recentUploadList: [],
      recentUploadStatus: false,
      recentUploadPage: 1,
      recentUploadPageLength: 1,
    };
  },
  methods: {
    GetRecentUploads(page) {
      if (!this.recentUploadStatus) {
        // Change the show recent uploads button to hide
        this.recentUploadStatus = true;

        // Fetch the RecentUploads
        fetch(
          `${global.ApiDomainName}/api/Recentuploads/${this.$store.state.userData.username}/${page}?ipAddress=${this.$store.state.userData.ipAddress}`
        )
          .then((response) => {
            // Display error view based on response status code
            global.ErrorHandler(this.$router, response);
            return response.json();
          })
          .then((data) => {
            // Loop the JSON array and update the view
            data.forEach((i) => {
              this.recentUploadList.push(i);
            });
          });

        // Fetch the pagination size.
        fetch(
          `${global.ApiDomainName}/api/RecentUploadPagination/${this.$store.state.userData.username}?ipAddress=${this.$store.state.userData.ipAddress}`
        )
          .then((response) => {
            // Display error view based on response status code
            global.ErrorHandler(this.$router, response);
            return response.json();
          })
          .then((data) => {
            // Update the pagination
            this.recentUploadPageLength = data;
          });
      } else {
        // When the button is closed update message and empty results.
        this.recentUploadStatus = false;
        this.recentUploadList = [];
      }
    },

    GetInProgressUploads(page) {
      if (!this.inProgressStatus) {
        // Change the show InProgress uploads button to hide
        this.inProgressStatus = true;

        // Fetch the InProgress uploads
        fetch(
          `${global.ApiDomainName}/api/InProgressUploads/${this.$store.state.userData.username}/${page}?ipAddress=${this.$store.state.userData.ipAddress}`
        )
          .then((response) => {
            // Display error view based on response status code
            global.ErrorHandler(this.$router, response);
            return response.json();
          })
          .then((data) => {
            // Loop the JSON array and update the view
            data.forEach((i) => {
              this.inProgressList.push(i);
            });
          });

        // Fetch the pagination size.
        fetch(
          `${global.ApiDomainName}/api/InProgressUploadPagination/${this.$store.state.userData.username}?ipAddress=${this.$store.state.userData.ipAddress}`
        )
          .then((response) => {
            // Display error view based on response status code
            global.ErrorHandler(this.$router, response);
            return response.json();
          })
          .then((data) => {
            // Update the pagination
            this.recentUploadPageLength = data;
          });
      } else {
        // When the button is closed update message and empty results.
        this.inProgressStatus = false;
        this.inProgressList = [];
      }
    },

    GetSaveList(page) {
      if (!this.saveListStatus) {
        // Change the show saveList button to hide
        this.saveListStatus = true;

        // Fetch the save lists for a user
        fetch(
          `${global.ApiDomainName}/api/SaveList/${this.$store.state.userData.username}/${page}?ipAddress=${this.$store.state.userData.ipAddress}`
        )
          .then((response) => {
            // Display error view based on response status code
            global.ErrorHandler(this.$router, response);
            return response.json();
          })
          .then((data) => {
            // Loop the JSON array and update the view
            data.forEach((i) => {
              this.saveList.push(i);
            });
          });

        // Fetch the pagination size.
        fetch(
          `${global.ApiDomainName}/api/SaveListPagination/${this.$store.state.userData.username}?ipAddress=${this.$store.state.userData.ipAddress}`
        )
          .then((response) => {
            // Display error view based on response status code
            global.ErrorHandler(this.$router, response);
            return response.json();
          })
          .then((data) => {
            // Update the pagination
            this.recentUploadPageLength = data;
          });
      } else {
        // When the button is closed update message and empty results.
        this.saveListStatus = false;
        this.saveList = [];
      }
    },
  },
  watch: {
    saveListPage(newValue, oldValue) {
      // Recall fetch save list with new pagination.
      fetch(
        `${global.ApiDomainName}/api/SaveList/${
          this.$store.state.userData.username
        }/${newValue - 1}?ipAddress=${this.$store.state.userData.ipAddress}`
      )
        .then((response) => {
          // Display error view based on response status code
          global.ErrorHandler(this.$router, response);
          return response.json();
        })
        .then((data) => {
          // Add the page of the savelist to the saveList data object
          this.saveList = [];
          data.forEach((i) => {
            this.saveList.push(i);
          });
        });
    },
    inProgressPage(newValue, oldValue) {
      // Recall fetch in progress with new pagination.
      fetch(
        `${global.ApiDomainName}/api/InProgressUploads/${
          this.$store.state.userData.username
        }/${newValue - 1}?ipAddress=${this.$store.state.userData.ipAddress}`
      )
        .then((response) => {
          // Display error view based on response status code
          global.ErrorHandler(this.$router, response);
          return response.json();
        })
        .then((data) => {
          // Reset the in progress list for new pagination.
          this.inProgressList = [];
          data.forEach((i) => {
            this.inProgressList.push(i);
          });
        });
    },
    recentUploadPage(newValue, oldValue) {
      // Recall fetch in progress for new pagination.
      fetch(
        `${global.ApiDomainName}/api/Recentuploads/${
          this.$store.state.userData.username
        }/${newValue - 1}?ipAddress=${this.$store.state.userData.ipAddress}`
      )
        .then((response) => {
          // Display error view based on response status code
          global.ErrorHandler(this.$router, response);
          return response.json();
        })
        .then((data) => {
          // Reset the recent upload list when page changes.
          this.recentUploadList = [];
          data.forEach((i) => {
            this.recentUploadList.push(i);
          });
        });
    },
  },
  created() {
    // Retrieve user name from the vuex store.
    this.username = this.$store.getters.username;

    // Make an API call to calculate the score for this user \.
    fetch(
      `${global.ApiDomainName}/api/ProfileScore/${this.$store.state.userData.username}?ipAddress=${this.$store.state.userData.ipAddress}`
    )
      .then((response) => {
        // Display error view based on response status code
        global.ErrorHandler(this.$router, response);
        return response.json();
      })
      .then((data) => {
        // Loop through the array and sum scores
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
