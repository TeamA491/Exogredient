<template>
  <div class="section">
    <div class="columns">
      <!-- SIDE NAV -->
      <aside class="hero is-fullheight-with-navbar column is-2" id="sidebar">
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
        </nav>
      </aside>

      <!-- MAIN VIEW -->
      <div class="column">
        <h1 class="title is-1">Tickets</h1>

        <!-- NO TICKETS MESSAGE -->
        <div class="hero" id="no-tickets-display">
          <div class="hero-body">
            <div class="container has-text-centered">
              <h1 class="title">No Tickets</h1>
              <h2 class="subtitle">View all your tickets here</h2>
            </div>
          </div>
        </div>

        <!-- TICKETS TABLE -->
        <table class="table is-hoverable is-fullwidth is-hidden" id="tickets-table">
          <thead>
            <tr>
              <th>
                <input type="checkbox" id="select-all-checkbox" />
              </th>
              <th>
                <abbr title="Ticket ID">ID</abbr>
              </th>
              <th>Flag Color</th>
              <th>Read Status</th>
              <th>Category</th>
            </tr>
          </thead>

          <tfoot>
            <th></th>
            <th>
              <abbr title="Ticket ID">ID</abbr>
            </th>
            <th>Flag Color</th>
            <th>Read Status</th>
            <th>Category</th>
          </tfoot>

          <tbody>
            <tr v-for="ticket in tickets" :key="ticket.id">
              <th>
                <input type="checkbox" id="select-all-checkbox" />
              </th>
              <td>{{ ticket.id }}</td>
              <td>{{ ticket.flagColor }}</td>
              <td>{{ ticket.readStatus }}</td>
              <td>{{ ticket.category }}</td>
            </tr>
          </tbody>
        </table>

      <!-- PAGINATION -->
        <!-- <nav class="pagination" role="navigation" aria-label="pagination" id="pagination">
          <a class="pagination-previous">Previous</a>
          <a class="pagination-next">Next page</a>
          <ul class="pagination-list">
            <li>
              <a class="pagination-link" aria-label="Goto page 1">1</a>
            </li>
            <li>
              <span class="pagination-ellipsis">&hellip;</span>
            </li>
            <li>
              <a class="pagination-link" aria-label="Goto page 45">45</a>
            </li>
            <li>
              <a class="pagination-link is-current" aria-label="Page 46" aria-current="page">46</a>
            </li>
            <li>
              <a class="pagination-link" aria-label="Goto page 47">47</a>
            </li>
            <li>
              <span class="pagination-ellipsis">&hellip;</span>
            </li>
            <li>
              <a class="pagination-link" aria-label="Goto page 86">86</a>
            </li>
          </ul>
        </nav> -->

        <!-- CURRENT TICKET VIEW -->
        <div class="container is-fluid has-background-grey-lighter is-hidden" id="cur-ticket-view">
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
</template>

<!-- ================================================= -->

<script>
import Vue from "vue";

export default {
  name: "tickets-view",
  data() {
    return {
      tickets: []
    };
  }
};

var statusDropdown;
var categoryDropdown;
var flagColorsDropdown;
var readStatusDropdown;

var ticketsTable;
var noTicketsDisplay;
var pagination;

var curTicketView;
var curTicketFlagColorsDropdown;
var curTicketCategoryDropdown;
var curTicketMarkReadButton;
var curTicketMarkResolvedButton;
var curTicketBackButton;

// On document ready...
document.addEventListener("DOMContentLoaded", function() {
  // Initializing
  statusDropdown = document.getElementById("status-dropdown");
  categoryDropdown = document.getElementById("category-dropdown");
  flagColorsDropdown = document.getElementById("flag-colors-dropdown");
  readStatusDropdown = document.getElementById("read-status-dropdown");
  // pagination = document.getElementById("pagination");
  curTicketMarkReadButton = document.getElementById(
    "cur-ticket-mark-read-button"
  );
  curTicketMarkResolvedButton = document.getElementById(
    "cur-ticket-mark-resolved-button"
  );

  noTicketsDisplay = document.getElementById("no-tickets-display");
  ticketsTable = document.getElementById("tickets-table");

  curTicketView = document.getElementById("cur-ticket-view");
  curTicketFlagColorsDropdown = document.getElementById(
    "cur-ticket-flag-colors"
  );
  curTicketCategoryDropdown = document.getElementById(
    "cur-ticket-category-dropdown"
  );
  curTicketBackButton = document.getElementById("cur-ticket-back-button");

  // Add event listeners
  statusDropdown.addEventListener("change", onStatusDropdownChange);
  categoryDropdown.addEventListener("change", onCategoryDropdownChange);
  flagColorsDropdown.addEventListener("change", onFlagColorsDropdownChange);
  readStatusDropdown.addEventListener("change", readStatusDropdown);

  curTicketFlagColorsDropdown.addEventListener(
    "change",
    onCurTicketFlagColorsDropdownChange
  );
  curTicketCategoryDropdown.addEventListener(
    "change",
    onCurTicketCategoryDropdownChange
  );

  curTicketMarkReadButton.addEventListener(
    "click",
    onCurTicketMarkReadButtonClick
  );
  curTicketMarkResolvedButton.addEventListener(
    "click",
    onCurTicketMarkResolvedButtonClick
  );
  curTicketBackButton.addEventListener("click", onCurTicketBackButtonClick);

  // Check if there are tickets or not
  // displayTicketView();
  hideTable();
  hideTicketView();
});

/*
******************
CHANGE EVENTS
******************
*/
function onStatusDropdownChange() {}

function onCategoryDropdownChange() {}

function onFlagColorsDropdownChange() {}

function onReadStatusDropdownChange() {}

function onCurTicketFlagColorsDropdownChange() {}

function onCurTicketCategoryDropdownChange() {}

/*
******************
CLICK EVENTS
******************
*/

function onCurTicketMarkReadButtonClick() {}

function onCurTicketMarkResolvedButtonClick() {}

function onCurTicketBackButtonClick() {
  hideTicketView();
  // Show the table because if we were looking at a ticket, then
  // that means there was at least one ticket
  showTable();
}

/*
******************
OTHER
******************
*/
function showTable() {
  noTicketsDisplay.classList.add("is-hidden");
  ticketsTable.classList.remove("is-hidden");
}

function hideTable() {
  ticketsTable.classList.add("is-hidden");
  noTicketsDisplay.classList.remove("is-hidden");
}

function displayTicketView() {
  curTicketView.classList.remove("is-hidden");
}

function hideTicketView() {
  curTicketView.classList.add("is-hidden");
}
</script>

<!-- ================================================= -->

<style scoped>
#sidebar {
  background-color: #fafafa;
}
#divider {
  background-color: darkgrey;
}
</style>
