import Vue from 'vue'
import VueRouter from 'vue-router'
import SearchResultsView from '../views/SearchResultsView';
import StoreView from '../views/StoreView';
import ErrorView from '../views/ErrorView';

Vue.use(VueRouter)

const routes = [
  { path: '/SearchResultsView', component: SearchResultsView},
  { path: '/StoreView', component: StoreView},
  { path: '/ErrorView', component: ErrorView}, 

  {
    path: "/profile",
    name: "profile",
    component: () => import("@/pages/UserProfile.vue"),
  },
  {
    path: "/upload",
    name: "upload",
    component: () => import("@/pages/Upload.vue"),
  },
  {
    path: "/",
    name: "home",
  },
  
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
});


export default router
