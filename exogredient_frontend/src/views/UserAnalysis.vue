<template>
  <div id="UserAnalysis">
    <h1>User Analysis Dashboard</h1>
    <div class="table-option-container">
      <v-row>
        <v-col>
          <v-select id="yearsTable" label="Years" v-model="yearTable" :items="yearList"></v-select>
        </v-col>
        <v-col>
          <v-select id="monthsTable" label="Months" v-model="monthTable" :items="monthList"></v-select>
        </v-col>
      </v-row>
      <button @click="GetSingleSnapshotTable();">Submit</button>
    </div>

    <div class="table-container">
      <table class="userRegisteredTable">
        <caption>Amount of Registered Users</caption>
        <tr>
          <th>User Type</th>
          <th>Amount</th>
        </tr>
        <tr v-for="user in registeredUsers" v-bind:key="user.name">
          <td>{{user.name}}</td>
          <td>{{user.value}}</td>
        </tr>
      </table>

      <table class="userUploadedTable">
        <caption>Top 10 users with most Upload</caption>
        <tr>
          <th></th>
          <th>Username</th>
          <th>Amount</th>
        </tr>
        <tr v-for="(user, index) in topUsersUploaded" v-bind:key="user.name">
          <td>{{index+1}}</td>
          <td>{{user.name}}</td>
          <td>{{user.value}}</td>
        </tr>
      </table>

      <table class="userUpvoteTable">
        <caption>Top 10 users with most Upvote</caption>
        <tr>
          <th></th>
          <th>Username</th>
          <th>Amount</th>
        </tr>
        <tr v-for="(user, index) in topUsersUpvoted" v-bind:key="user.name">
          <td>{{index+1}}</td>
          <td>{{user.name}}</td>
          <td>{{user.value}}</td>
        </tr>
      </table>

      <table class="userdownVoteTable">
        <caption>Top 10 users with most downvote</caption>
        <tr>
          <th></th>
          <th>Username</th>
          <th>Amount</th>
        </tr>
        <tr v-for="(user, index) in topUsersDownvoted" v-bind:key="user.name">
          <td>{{index+1}}</td>
          <td>{{user.name}}</td>
          <td>{{user.value}}</td>
        </tr>
      </table>

      <table class="cityTable">
        <caption>Top 10 cities using application</caption>
        <tr>
          <th></th>
          <th>City Name</th>
          <th>Amount</th>
        </tr>
        <tr v-for="(city, index) in topCities" v-bind:key="city.name">
          <td>{{index+1}}</td>
          <td>{{city.name}}</td>
          <td>{{city.value}}</td>
        </tr>
      </table>

      <table class="searchedIngredientTable">
        <caption>Top 10 searched Ingredient</caption>
        <tr>
          <th></th>
          <th>Ingredient Name</th>
          <th>Amount</th>
        </tr>
        <tr v-for="(ingredient, index) in topIngredientSearched" v-bind:key="ingredient.name">
          <td>{{index+1}}</td>
          <td>{{ingredient.name}}</td>
          <td>{{ingredient.value}}</td>
        </tr>
      </table>

      <table class="searchedStoreTable">
        <caption>Top 10 searched Store</caption>
        <tr>
          <th></th>
          <th>Store Name</th>
          <th>Amount</th>
        </tr>
        <tr v-for="(store, index) in topStoreSearched" v-bind:key="store.name">
          <td>{{index+1}}</td>
          <td>{{store.name}}</td>
          <td>{{store.value}}</td>
        </tr>
      </table>

      <table class="uploadedIngredientTable">
        <caption>Top 10 uploaded Ingredient</caption>
        <tr>
          <th></th>
          <th>Ingredient Name</th>
          <th>Amount</th>
        </tr>
        <tr v-for="(ingredient, index) in topIngredientUploaded" v-bind:key="ingredient.name">
          <td>{{index+1}}</td>
          <td>{{ingredient.name}}</td>
          <td>{{ingredient.value}}</td>
        </tr>
      </table>

      <table class="uploadedStoreTable">
        <caption>Top 10 Store with most uploads</caption>
        <tr>
          <th></th>
          <th>Store Name</th>
          <th>Amount</th>
        </tr>
        <tr v-for="(store, index) in topStoreUploaded" v-bind:key="store.name">
          <td>{{index+1}}</td>
          <td>{{store.name}}</td>
          <td>{{store.value}}</td>
        </tr>
      </table>
    </div>
    <div class="graph-container">
      <div class="graph-option-container">
        <v-row>
          <v-col>
            <v-select id="yearsChart" label="Years" v-model="yearChart" :items="yearList"></v-select>
          </v-col>
          <v-col>
            <v-select id="monthsChart" label="Months" v-model="monthChart" :items="monthList"></v-select>
          </v-col>
          <v-col>
            <v-select
              id="operations"
              label="Operations"
              v-model="operationName"
              :items="operationLists"
            ></v-select>
          </v-col>
        </v-row>
        <button @click="GetSingleSnapshotChart();">Submit</button>
      </div>
      <canvas id="singleChart"></canvas>
    </div>

    <v-dialog v-model="dialog" max-width="800" :persistent="true">
      <v-card>
        <v-card-title class="headline">Error Message</v-card-title>
        <v-card-text>{{ DialogMessage }}</v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>

          <v-btn @click.stop="CloseMessage()">Close</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>>


<script>
import * as global from "../globalExports.js";

export default {
  data() {
    return {
      dialog: false,
      DialogMessage: "",
      yearList: [],
      monthList: [1, 2, 3, 4],
      yearMonthList: [],
      operationLists: [
        "Downvote Upload",
        "Upvote Upload",
        "Creating Upload",
        "Searching",
        "Registration",
        "Sleep"
      ],
      monthTable: 0,
      yearTable: 0,
      monthTableHolder: 0,
      yearTableHolder: 0,
      monthChart: 0,
      yearChart: 0,
      monthChartHolder: 0,
      yearChartHolder: 0,
      snapshotExistance: false,
      operations: [],
      registeredUsers: [],
      topCities: [],
      topUsersUploaded: [],
      topIngredientUploaded: [],
      topStoreUploaded: [],
      topIngredientSearched: [],
      topStoreSearched: [],
      topUsersUpvoted: [],
      topUsersDownvoted: [],
      operationName: "Registration",
      operationExist: false,
      singleChartExist: false,
      date: [],
      specificOperationSucc: [],
      specificOperationFail: [],
      specificOperationTotal: [],
      chartXLabel : "Dates",
      myChart: Chart
    };
  },
  // Start the view with displaying the tables and graph for previous month.
  created() {
    var today = new Date();
    var year = today.getFullYear();
    var month = today.getMonth() + 1;

    this.yearTable = year;
    this.yearChart = year;
    if (month > 1) {
      this.monthTable = month - 1;
      this.monthChart = month - 1;
    } else {
      this.monthTable = 12;
      this.yearTable = year - 1;
      this.monthChart = 12;
      this.yearChart = year - 1;
    }
    //Get the years with snapshots to set yearList.
    this.GetYear();
    //Get snapshot for tables.
    this.GetSingleSnapshotTable();
    //Get snapshot for charts.
    this.GetSingleSnapshotChart();
  },

  methods: {
    // Method to close the error popup.
    CloseMessage() {
      this.dialog = false;
    },

    // Method to fetch year and month data from backend.
    GetYear() {
      fetch(
        `${global.ApiDomainName}/api/FetchYearMonth`
      )
      .then(response => {
        return response.json();
      })
      .then(yearMonth => {
        this.yearMonthList = this.Format(yearMonth);
        this.SetYearMonthList();
      })
      .catch(err => {
        this.DialogMessage = `Unable to retrieve the years and months.`;
        this.dialog = true;
      });
    },

    // Method to set the year list.
    Format(formattedStr) {
      var myJSON = JSON.stringify(formattedStr);
      myJSON = myJSON.substring(14);
      myJSON = myJSON.substring(0, myJSON.length - 2);
      var parse1 = myJSON.replace(/'/g, '"');
      return JSON.parse(parse1);
    },

    // Method to set the year list and month list on start.
    SetYearMonthList() {
      for (var i = 0; i < this.yearMonthList.length; i++) {
        this.yearList.push(this.yearMonthList[i].name);
      }
      this.monthList = [""];
      var latestYear = this.yearMonthList.length-1;
      this.yearMonthList[latestYear].value.forEach(month => {
        this.monthList.push(month);
      });
    },

    // Method to set only the month list.
    setMonthList(year) {
      this.monthList = [""];
      var index = 0;

      for (var i = 0; i < this.yearMonthList.length; i++) {
        if (year == this.yearMonthList[i].name) {
          index = i;
        }
      }
      this.yearMonthList[index].value.forEach(month => {
        this.monthList.push(month);
      });
    },

    // Method to get a snapshot for the tables.
    GetSingleSnapshotTable() {
      var specificMonth = this.yearTable + "/" + this.monthTable;
      var specificMonthHolder = this.yearTableHolder + "/" + this.monthTableHolder;
      if (specificMonth != specificMonthHolder) {
        this.yearTableHolder = this.yearTable;
        this.monthTableHolder = this.monthTable;
        if (this.monthTable == "") {
          fetch(
            `${global.ApiDomainName}/api/FetchMulti/${this.yearTable}`
          )
          .then(response => {
            return response.json();
          })
          .then(snapshot => {
            this.registeredUsers = this.FormatTableData(
              snapshot.count_of_registered_users
            );
            this.topCities = this.FormatTableData(
              snapshot.top_cities_that_uses_application
            );
            this.topUsersUploaded = this.FormatTableData(
              snapshot.top_users_that_upload
            );
            this.topIngredientUploaded = this.FormatTableData(
              snapshot.top_most_uploaded_ingredients
            );
            this.topStoreUploaded = this.FormatTableData(
              snapshot.top_most_uploaded_stores
            );
            this.topIngredientSearched = this.FormatTableData(
              snapshot.top_most_searched_ingredients
            );
            this.topStoreSearched = this.FormatTableData(
              snapshot.top_most_searched_stores
            );
            this.topUsersUpvoted = this.FormatTableData(
              snapshot.top_most_upvoted_users
            );
            this.topUsersDownvoted = this.FormatTableData(
              snapshot.top_most_downvoted_users
            );
          })
          .catch(err => {
            this.DialogMessage = `Snapshot ${specificMonth} does not exist.`;
            this.dialog = true;
          });
        }
        else {
          fetch(
            `${global.ApiDomainName}/api/FetchSingle/${this.yearTable}/${this.monthTable}`
          )
          .then(response => {
            return response.json();
          })
          .then(snapshot => {
            this.registeredUsers = this.FormatTableData(
              snapshot.count_of_registered_users
            );
            this.topCities = this.FormatTableData(
              snapshot.top_cities_that_uses_application
            );
            this.topUsersUploaded = this.FormatTableData(
              snapshot.top_users_that_upload
            );
            this.topIngredientUploaded = this.FormatTableData(
              snapshot.top_most_uploaded_ingredients
            );
            this.topStoreUploaded = this.FormatTableData(
              snapshot.top_most_uploaded_stores
            );
            this.topIngredientSearched = this.FormatTableData(
              snapshot.top_most_searched_ingredients
            );
            this.topStoreSearched = this.FormatTableData(
              snapshot.top_most_searched_stores
            );
            this.topUsersUpvoted = this.FormatTableData(
              snapshot.top_most_upvoted_users
            );
            this.topUsersDownvoted = this.FormatTableData(
              snapshot.top_most_downvoted_users
            );
          })
          .catch(err => {
            this.DialogMessage = `Snapshot ${specificMonth} does not exist.`;
            this.dialog = true;
          });
        }    
      }
    },

    // Method to get a snapshot for the chart.
    GetSingleSnapshotChart() {
      var specificMonth = this.yearChart + "/" + this.monthChart;
      var specificMonthHolder = this.yearChartHolder + "/" + this.monthChartHolder;
      if (specificMonth != specificMonthHolder) {
        this.operations = [];
        this.yearChartHolder = this.yearChart;
        this.monthChartHolder = this.monthChart;
        if (this.monthChartHolder == "") {
          fetch(
            `${global.ApiDomainName}/api/FetchMulti/${this.yearChart}`
          )
            .then(response => {
              return response.json();
            })
            .then(snapshot => {
              this.operations = this.FormatTableData(snapshot.operations);
              this.chartXLabel = "Months";
              this.StartSingleChartCreation();
            })
            .catch(err => {
              this.DialogMessage = `Snapshot ${specificMonth} does not exist.`;
              this.dialog = true;
            });
        }
        else {
          fetch(
            `${global.ApiDomainName}/api/FetchSingle/${this.yearChart}/${this.monthChart}`
          )
            .then(response => {
              return response.json();
            })
            .then(snapshot => {
              this.operations = this.FormatTableData(snapshot.operations);
              this.chartXLabel = "Dates";
              this.StartSingleChartCreation();
            })
            .catch(err => {
              this.DialogMessage = `Snapshot ${specificMonth} does not exist.`;
              this.dialog = true;
            });
        }
      }
      else {
        this.StartSingleChartCreation();
      }     
    },

    // Method to format a string and eval to use values.
    FormatTableData(str) {
      var formattedStr = str.replace(/'/g, '"');
      return JSON.parse(formattedStr);
    },

    // Method to start creating a chart.
    StartSingleChartCreation(){
      this.CheckOperationExistance();
      if (this.operationExist) {
        if (this.singleChartExist) {
          this.DestroyBarChart();
        }
        this.CreateBarChartSingle();
      }
      else{
        this.DialogMessage = `Selected operation ${this.operationName} has no data.`;
        this.dialog = true;
      }
    },

    // Method to see if a operation exists.
    CheckOperationExistance() {
      // Check if the operation exits by interating through operations list
      this.operationExist = false;
      for (var i = 0; i < this.operations.length; i++) {
        if (this.operationName == this.operations[i].name) {
          this.operationExist = true;
          break;
        }
      }
    },

    // Destroy the chart.
    DestroyBarChart() {
        this.myChart = this.myChart.destroy();
    },

    // Method to create the chart.
    CreateBarChartSingle() {
      // Get data for specific operation.
      this.GetOperationData(this.operationName);
      // Create array for the amount of days in a month.
      this.CreateDateArray();
      var ctx = document.getElementById("singleChart");
      this.myChart = new Chart(ctx, {
        type: "bar",
        data: {
          labels: this.date,
          datasets: [
            {
              label: "Total",
              data: this.specificOperationTotal,
              backgroundColor: "rgb(145, 208, 255)",
              borderColor: "#0F000E"
            },
            {
              label: "Success",
              data: this.specificOperationSucc,
              backgroundColor: "rgb(7, 253, 86)",
              borderColor: "#0F000E"
            },
            {
              label: "Fail",
              data: this.specificOperationFail,
              backgroundColor: "rgb(254, 12, 52)",
              borderColor: "#0F000E"
            }
          ]
        },
        options: {
          title: {
            display: true,
            text: this.operationName
          },
          responsive: true,
          lineTension: 1,
          scales: {
            yAxes: [
              {
                ticks: {
                  beginAtZero: true,
                  padding: 25
                },
                scaleLabel: {
                  display: true,
                  labelString: "Amount"
                }
              }
            ],
            xAxes: [
              {
                scaleLabel: {
                  display: true,
                  labelString: this.chartXLabel
                }
              }
            ]
          }
        }
      });
      this.singleChartExist = true;
    },

    // Method to create an array of dates.
    CreateDateArray() {
      this.date = [];
      // The amount of days can be found by first getting the length of the operations success,fail, and total list.
      // Divide the value by 3 to get the amount of days.
      var amountOfDays =
        (this.specificOperationSucc.length +
          this.specificOperationFail.length +
          this.specificOperationTotal.length) /
        3;
      for (var i = 0; i < amountOfDays; i++) {
        this.date[i] = i + 1;
      }
    },

    // Method to split the operation data into 3 list: success, fail, and total. Based on index.
    GetOperationData(operationName) {
      this.specificOperationSucc = [];
      this.specificOperationFail = [];
      this.specificOperationTotal = [];
      var data;

      this.operations.forEach(element => {
        if (operationName == element.name) {
          data = element.value;
        }
      });

      for (var i = 0; i < data.length; i++) {
        if (i % 3 == 0) {
          this.specificOperationSucc.push(data[i]);
        } else if ((i - 1) % 3 == 0) {
          this.specificOperationFail.push(data[i]);
        } else {
          this.specificOperationTotal.push(data[i]);
        }
      }
    }
  }
};
</script>

<style>
h1 {
  text-align: center;
}
table,
th,
td {
  margin: 25px 25px 25px 25px;
  border: 1px solid black;
}
th,
td {
  padding: 5px;
  text-align: left;
}
.table-container {
  display: grid;
  justify-content: center;
  grid-template-columns: auto auto auto;
  padding: 10px;
}
.graph-container {
  display: block;
  width: 60%;
  margin: auto;
  padding-bottom: 50px;
}
.table-option-container {
  width: 50%;
  margin: auto;
}
.graph-option-container {
  width: 50%;
  margin: auto;
}
</style>