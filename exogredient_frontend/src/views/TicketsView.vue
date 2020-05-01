<template>
  <div class="section">
    <div class="columns">
      <!-- SIDE NAV -->
      <aside class="hero is-fullheight-with-navbar column is-2" id="sidebar">
        <nav class="menu">
          <p class="menu-label">Status</p>
          <ul class="menu-list">
            <li>
              <a class="is-active">Unresolved</a>
            </li>

            <li>
              <a>Resolved</a>
            </li>
          </ul>

          <p class="menu-label">Category</p>
          <div class="select is-fullwidth">
            <select>
              <option>All</option>
              <option>Bug</option>
              <option>Error</option>
              <option>Report</option>
              <option>Other</option>
            </select>
          </div>

          <p class="menu-label">Read Status</p>
          <div class="select is-fullwidth">
            <select>
              <option>All</option>
              <option>Read</option>
              <option>Unread</option>
            </select>
          </div>
        </nav>
      </aside>

      <!-- MAIN VIEW -->
      <div class="column">
        <!-- TOP OPTIONS -->
        <nav class="level">
          <div class="level-left">
            <!-- SELECT ALL CHECK BOX -->
            <div class="level-item">
              <button class="button">Date range</button>
              <span class="tag is-large">04/1/20 - 05/1/20</span>
            </div>

            <!-- FLAG COLOR SELECTION -->
            <div class="level-item">Flag Colors</div>
            <div class="level-item">
              <div class="buttons has-addons">
                <button class="button">all</button>
                <button class="button is-info is-selected">red</button>
                <button class="button">green</button>
                <button class="button">blue</button>
              </div>
            </div>
          </div>
        </nav>

        <!-- TICKETS TABLE -->
        <table class="table is-hoverable is-fullwidth" id="ticket-table">
          <thead>
            <tr>
              <th>
                <abbr title="Ticket ID">ID</abbr>
              </th>
              <th>Flag Color</th>
              <th>Read Status</th>
              <th>Category</th>
            </tr>
          </thead>

          <tfoot>
            <th>
              <abbr title="Ticket ID">ID</abbr>
            </th>
            <th>Flag Color</th>
            <th>Read Status</th>
            <th>Category</th>
          </tfoot>

          <tbody>
            <tr>
              <th>1</th>
              <td>2</td>
              <td>3</td>
              <td>Bug</td>
            </tr>
            <tr>
              <th>1</th>
              <td>2</td>
              <td>3</td>
              <td>Error</td>
            </tr>
            <tr>
              <th>1</th>
              <td>2</td>
              <td>3</td>
              <td>Report</td>
            </tr>
            <tr>
              <th>1</th>
              <td>2</td>
              <td>3</td>
              <td>Bug</td>
            </tr>
          </tbody>
        </table>

        <!-- PAGINATION -->
        <nav class="pagination" role="navigation" aria-label="pagination">
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
        </nav>
      </div>
    </div>
  </div>
</template>

<!-- ================================================= -->

<script lang="ts">
import Vue from "vue";

export default Vue.extend({
  name: "tickets-view"
});
</script>

<!-- ================================================= -->

<style scoped>
#sidebar {
  background-color: #fafafa;
}
</style>
