<template>
  <div>
    <div class="modal" ref="curTicketModal">
      <div class="modal-background"></div>
      <div class="modal-card">
        <header class="modal-card-head">
          <p class="modal-card-title">Modal title</p>
          <button class="delete" aria-label="close" @click="hideTicketView"></button>
        </header>
        <section class="modal-card-body">
          <!-- Content ... -->
        </section>
        <footer class="modal-card-foot">
          <button class="button is-success" @click="hideTicketView">Ok</button>
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
              <select id="status-dropdown">
                <option>Unresolved</option>
                <option>Resolved</option>
              </select>
            </div>

            <!-- CATEGORY DROPDOWN -->
            <p class="menu-label">Category</p>
            <div class="select is-fullwidth">
              <select id="category-dropdown">
                <option>All</option>
                <option>Bug</option>
                <option>Error</option>
                <option>Report</option>
                <option>Other</option>
              </select>
            </div>

            <!-- READ STATUS DROPDOWN -->
            <p class="menu-label">Read Status</p>
            <div class="select is-fullwidth">
              <select id="read-status-dropdown">
                <option>All</option>
                <option>Read</option>
                <option>Unread</option>
              </select>
            </div>

            <!-- FLAG COLOR DROPDOWN -->
            <p class="menu-label">Flag Color</p>
            <div class="select is-fullwidth">
              <select id="flag-colors-dropdown">
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
          <table class="table is-hoverable is-fullwidth" id="tickets-table" ref="ticketsTable">
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
              <tr v-for="ticket in tickets" :key="ticket.id" @click="displayTicketView">
                <td class="tableColumnSmall">{{ ticket.id }}</td>
                <td class="tableColumnLarge">{{ ticket.category }}</td>
                <td class="tableColumnLarge">{{ ticket.flagColor }}</td>
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
          <div
            class="container is-fluid has-background-grey-lighter"
            id="cur-ticket-view"
            ref="curTicketView"
          >
            <!-- CURRENT TICKET HEADER -->
            <div class="level">
              <div class="level-left">
                <!-- CURRENT TICKET CATEGORY DROPDOWN -->
                <div class="level-item has-text-centered">
                  <p>Category</p>
                  <div class="select">
                    <select id="cur-ticket-category-dropdown">
                      <option>Bug</option>
                      <option>Error</option>
                      <option>Report</option>
                      <option>Other</option>
                    </select>
                  </div>
                </div>

                <!-- CURRENT TICKET FLAG COLOR DROPDOWN -->
                <div class="level-item has-text-centered">
                  <p>Flag color</p>
                  <div class="select" id="cur-ticket-flag-colors">
                    <select>
                      <option>None</option>
                      <option>Red</option>
                      <option>Purple</option>
                      <option>Blue</option>
                      <option>Green</option>
                      <option>Orange</option>
                    </select>
                  </div>
                </div>
              </div>

              <div class="level-left">
                <!-- CURRENT TICKET READ STATUS TOGGLE BUTTON -->
                <div class="level-item has-text-centered">
                  <button class="button is-success" id="cur-ticket-mark-read-button">Mark as unread</button>
                </div>

                <!-- CURRENT TICKET STATUS BUTTON -->
                <div class="level-item has-text-centered">
                  <button
                    class="button is-success"
                    id="cur-ticket-mark-resolved-button"
                  >Mark as resolved</button>
                </div>
              </div>
            </div>

            <h3>Submitted: 05/01/2020</h3>

            <textarea readonly class="textarea" rows="10" id="current-ticket-description"></textarea>

            <button class="button" id="cur-ticket-back-button">Back</button>
          </div>
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
      tickets: [
        {
          id: 1,
          category: "bug",
          date: "05/12/20",
          flagColor: "red",
          isUnread: true,
        },
        {
          id: 2,
          category: "error",
          date: "02/23/20",
          flagColor: "orange",
          isUnread: false,
        },
        {
          id: 3,
          category: "suggestion",
          date: "01/08/20",
          flagColor: "blue",
          isUnread: false,
        }
      ]
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
    displayTicketView: function() {
      this.$refs.curTicketModal.classList.add("is-active");
    },
    hideTicketView: function() {
      this.$refs.curTicketModal.classList.remove("is-active");
    },
    onSearchButtonClick: function() {
      this.$refs.searchButton.classList.add("is-loading");

      setTimeout(() => {
        this.$refs.searchButton.classList.remove("is-loading");
      }, 2000);
    }
  },
  mounted: function() {
    this.$refs.noTicketsDisplay.classList.add("is-hidden");
    this.$refs.ticketsTable.classList.remove("is-hidden");
    this.$refs.curTicketView.classList.add("is-hidden");

    // showTable();
    // hideTicketView();
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
  width: 15px;
  height: 15px;
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
</style>
