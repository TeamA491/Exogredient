<template>
  <div>
    <!-- MODAL -->
    <div class="modal" ref="curTicketModal">
      <div class="modal-background"></div>
      <div class="modal-card">
        <!-- MODAL HEADER -->
        <header class="modal-card-head">
          <div class="field">
            <div class="control has-icons-left">
              <div class="select">
                <select
                  id="cur-ticket-flag-dropdown"
                  ref="curTicketFlagDropdown"
                  @change="onCurTicketFlagDropdownChange"
                >
                  <option>Select Flag Color</option>
                  <option>Red</option>
                  <option>Purple</option>
                  <option>Blue</option>
                  <option>Green</option>
                  <option>Orange</option>
                </select>
              </div>
              <div class="icon is-small is-left">
                <img :src="noColorFlag" class="flagIcon" />
              </div>
            </div>
          </div>
        </header>

        <!-- MODAL BODY -->
        <section class="modal-card-body">
          <textarea
            readonly
            class="textarea"
            rows="10"
            id="current-ticket-description"
            ref="curTicketDescription"
          ></textarea>
          <h3 ref="curTicketDate"></h3>
        </section>

        <!-- MODAL FOOTER -->
        <footer class="modal-card-foot">
          <button class="button" @click="hideTicketModal">Close</button>

          <!-- CURRENT TICKET READ STATUS TOGGLE BUTTON -->
          <button
            class="button is-primary"
            id="cur-ticket-mark-read-button"
            ref="curTicketMarkReadButton"
            @click="toggleCurTicketReadStatus"
          >Mark as unread</button>

          <!-- CURRENT TICKET STATUS BUTTON -->
          <button
            class="button is-primary"
            id="cur-ticket-mark-resolved-button"
            ref="curTicketMarkResolvedButton"
            @click="toggleCurTicketStatus"
          >Mark as resolved</button>
        </footer>
      </div>
    </div>

    <div class="section">
      <div class="columns">
        <!-- SIDE NAV -->
        <aside class="hero column is-2" id="sidebar">
          <nav class="menu">
            <h1 class="title">Search Filters</h1>

            <hr class="divider" />

            <!-- STATUS DROPDOWN -->
            <p class="menu-label">Status</p>
            <div class="select is-fullwidth">
              <select id="status-dropdown" ref="statusDropdown">
                <option>Unresolved</option>
                <option>Resolved</option>
              </select>
            </div>

            <!-- CATEGORY DROPDOWN -->
            <p class="menu-label">Category</p>
            <div class="select is-fullwidth">
              <select id="category-dropdown" ref="categoryDropdown">
                <option>All</option>
                <option>Bug</option>
                <option>Error</option>
                <option>Suggestion</option>
                <option>Other</option>
              </select>
            </div>

            <!-- READ STATUS DROPDOWN -->
            <p class="menu-label">Read Status</p>
            <div class="select is-fullwidth">
              <select id="read-status-dropdown" ref="readStatusDropdown">
                <option>All</option>
                <option>Read</option>
                <option>Unread</option>
              </select>
            </div>

            <!-- FLAG COLOR DROPDOWN -->
            <p class="menu-label">Flag Color</p>
            <div class="select is-fullwidth">
              <select id="flag-colors-dropdown" ref="flagColorsDropdown">
                <option>All</option>
                <option>Red</option>
                <option>Purple</option>
                <option>Blue</option>
                <option>Green</option>
                <option>Orange</option>
              </select>
            </div>

            <p class="menu-label"></p>
            <button
              class="button is-fullwidth is-primary"
              ref="searchButton"
              @click="onSearchButtonClick"
            >Search</button>
          </nav>
        </aside>

        <!-- MAIN VIEW -->
        <div class="column">
          <h1 class="title is-1">Tickets</h1>

          <!-- NO TICKETS MESSAGE -->
          <div class="hero" id="no-tickets-display" ref="noTicketsDisplay">
            <div class="hero-body">
              <div class="container has-text-centered">
                <h1 class="title">No Tickets</h1>
                <h2 class="subtitle">View all your tickets here</h2>
              </div>
            </div>
          </div>

          <!-- TICKETS TABLE -->
          <table
            class="table is-hoverable is-fullwidth is-hidden"
            id="tickets-table"
            ref="ticketsTable"
          >
            <thead>
              <tr>
                <th class="tableColumnSmall">
                  <abbr title="Ticket ID">ID</abbr>
                </th>
                <th class="tableColumnLarge">Category</th>
                <th class="tableColumnLarge">Flag</th>
                <th class="tableColumnLarge">Date</th>
                <th class="unreadNotifColumn"></th>
              </tr>
            </thead>

            <tfoot>
              <th class="tableColumnSmall">
                <abbr title="Ticket ID">ID</abbr>
              </th>
              <th class="tableColumnLarge">Category</th>
              <th class="tableColumnLarge">Flag</th>
              <th class="tableColumnLarge">Date</th>
              <th class="unreadNotifColumn"></th>
            </tfoot>

            <tbody>
              <tr v-for="ticket in tickets" :key="ticket.id" @click="onTicketPress">
                <td class="tableColumnSmall">{{ ticket.id }}</td>
                <td class="tableColumnLarge">{{ ticket.category }}</td>

                <td class="tableColumnLarge">
                  <img v-if="ticket.flagColor === 'Red'" :src="redFlag" class="flagIcon" />
                  <img v-else-if="ticket.flagColor === 'Purple'" :src="purpleFlag" class="flagIcon" />
                  <img v-else-if="ticket.flagColor === 'Blue'" :src="blueFlag" class="flagIcon" />
                  <img v-else-if="ticket.flagColor === 'Green'" :src="greenFlag" class="flagIcon" />
                  <img v-else-if="ticket.flagColor === 'Orange'" :src="orangeFlag" class="flagIcon" />
                </td>

                <td class="tableColumnLarge">{{ ticket.date }}</td>
                <th>
                  <div v-if="ticket.isUnread">
                    <span class="unreadNotificationDot"></span>
                  </div>
                </th>
              </tr>
            </tbody>
          </table>

          <!-- CURRENT TICKET VIEW -->
        </div>
      </div>
    </div>
  </div>
</template>

<!-- ================================================= -->

<script>
import Vue from "vue";

export default {
  name: "tickets-view",
  data() {
    return {
      tickets: [],
      curTicket: {},
      redFlag: require("@/assets/flag_icons/flag_icon_red.png"),
      blueFlag: require("@/assets/flag_icons/flag_icon_blue.png"),
      greenFlag: require("@/assets/flag_icons/flag_icon_green.png"),
      orangeFlag: require("@/assets/flag_icons/flag_icon_orange.png"),
      purpleFlag: require("@/assets/flag_icons/flag_icon_purple.png"),
      noColorFlag: require("@/assets/flag_icons/flag_icon.png")
    };
  },
  methods: {
    showTable: function() {
      this.$refs.noTicketsDisplay.classList.add("is-hidden");
      this.$refs.ticketsTable.classList.remove("is-hidden");
    },
    hideTable: function() {
      this.$refs.ticketsTable.classList.add("is-hidden");
      this.$refs.noTicketsDisplay.classList.remove("is-hidden");
    },
    displayTicketModal: function() {
      this.$refs.curTicketModal.classList.add("is-active");
    },
    hideTicketModal: function() {
      this.$refs.curTicketModal.classList.remove("is-active");
    },
    toggleCurTicketReadStatus: function() {
      if (this.curTicket.isUnread) {
        this.setCurTicketAsRead();
      } else {
        this.setCurTicketAsUnread();
      }
    },
    setCurTicketAsRead: function() {
      this.curTicket.isUnread = false;
      this.$refs.curTicketMarkReadButton.innerHTML = "Mark as unread";
    },
    setCurTicketAsUnread: function() {
      this.curTicket.isUnread = true;
      this.$refs.curTicketMarkReadButton.innerHTML = "Mark as read";
    },
    setCurTicketAsResolved: function() {
      this.curTicket.status = "Resolved";
      this.$refs.curTicketMarkResolvedButton.innerHTML = "Mark as unresolved";
    },
    setCurTicketAsUnresolved: function() {
      this.curTicket.status = "Unresolved";
      this.$refs.curTicketMarkResolvedButton.innerHTML = "Mark as resolved";
    },
    toggleCurTicketStatus: function() {
      if (this.curTicket.status == "Unresolved") {
        this.setCurTicketAsResolved();
      } else {
        this.setCurTicketAsUnresolved();
      }
    },
    onCurTicketFlagDropdownChange: function() {
      if (this.$refs.curTicketFlagDropdown.selectedIndex == 0) {
        this.curTicket.flagColor = "";
      } else {
        this.curTicket.flagColor = this.$refs.curTicketFlagDropdown.options[
          this.$refs.curTicketFlagDropdown.selectedIndex
        ].text;
      }
    },
    onTicketPress: function(event) {
      // Find the ticket
      var ticketID = event.target.parentElement.firstElementChild.innerHTML;
      for (var i = 0; i < this.tickets.length; i++) {
        if (this.tickets[i].id == ticketID) {
          this.curTicket = this.tickets[i];
          this.setCurTicketAsRead();

          // Set proper resolved status
          if (this.$refs.curTicketMarkReadButton.status == "Unresolved")
            this.setCurTicketAsUnresolved();
          else this.setCurTicketAsResolved();

          break;
        }
      }

      if (this.curTicket === undefined) return;

      // Set the proper ticket information
      this.$refs.curTicketDescription.innerHTML = this.curTicket.description;
      this.$refs.curTicketDate.innerHTML = new Date(
        this.curTicket.date * 1000
      ).toLocaleString();

      // Select the ticket flag value
      this.$refs.curTicketFlagDropdown.selectedIndex = 0;
      for (var i = 0; i < this.$refs.curTicketFlagDropdown.length; i++) {
        if (
          this.$refs.curTicketFlagDropdown.options[i].text ===
          this.curTicket.flagColor
        ) {
          this.$refs.curTicketFlagDropdown.selectedIndex = i;
          break;
        }
      }

      this.displayTicketModal();
    },
    onSearchButtonClick: function() {
      this.$refs.searchButton.classList.add("is-loading");

      var useFlagColors = this.$refs.flagColorsDropdown.selectedIndex != 0;
      var useCategory = this.$refs.categoryDropdown.selectedIndex != 0;
      var useReadStatus = this.$refs.readStatusDropdown.selectedIndex != 0;

      var flagColor = this.$refs.flagColorsDropdown.options[
        this.$refs.flagColorsDropdown.selectedIndex
      ].text;
      var category = this.$refs.categoryDropdown.options[
        this.$refs.categoryDropdown.selectedIndex
      ].text;
      var readStatus = this.$refs.readStatusDropdown.options[
        this.$refs.readStatusDropdown.selectedIndex
      ].text;
      var status = this.$refs.statusDropdown.options[
        this.$refs.statusDropdown.selectedIndex
      ].text;

      var getURL = `${global.ApiDomainName}/api/ticket?Status=${status}`;
      if (useFlagColors) {
        getURL += `&FlagColor=${flagColor}`;
      }
      if (useCategory) {
        getURL += `&Category=${category}`;
      }
      if (useReadStatus) {
        getURL += `&ReadStatus=${readStatus}`;
      }

      fetch(getURL).then(res => {
        var data = res.json();
        this.tickets = data;
        this.$refs.searchButton.classList.remove("is-loading");
        if (this.tickets.length == 0) this.hideTable();
        else this.showTable();
      });
    }
  },
  mounted: function() {
    this.hideTable();

    // Load the initial tickets
    fetch(`${global.ApiDomainName}/api/ticket?Status=${status}`).then(res => {
      var data = res.json();
      this.tickets = data;
      this.$refs.searchButton.classList.remove("is-loading");
      if (this.tickets.length == 0) this.hideTable();
      else this.showTable();
    });
  }
};
</script>

<!-- ================================================= -->

<style scoped>
#sidebar {
  background-color: #fafafa;
}
#divider {
  background-color: darkgrey;
}

.unreadNotificationDot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  display: inline-block;
  background-color: #35baf2;
}
.tableColumnLarge {
  width: 30%;
}
.tableColumnSmall {
  width: 10%;
}
.flagIcon {
  height: 20px;
  width: 20px;
}
</style>
