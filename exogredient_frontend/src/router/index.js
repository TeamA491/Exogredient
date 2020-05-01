import Vue from 'vue'
import VueRouter from 'vue-router'
import SearchResultsView from '../views/SearchResultsView';
import StoreView from '../views/StoreView';
import ErrorView from '../views/ErrorView';
import RegistrationView from '../views/RegistrationView';

Vue.use(VueRouter)

const routes = [
  { path: '/SearchResultsView', component: SearchResultsView},
  { path: '/StoreView', component: StoreView},
  { path: '/ErrorView', component: ErrorView}, 
  { path: '/RegistrationView', component: RegistrationView },
  { 
    path: '/verify',
    name: 'verify',
    component: () => import('../views/VerificationView.vue')
  },

  {
    path: "/profile",
    name: "profile",
    component: () => import("@/views/UserProfile.vue"),
  },
  {
    path: "/upload",
    name: "upload",
    component: () => import("@/views/Upload.vue"),
  },
  {
    path: "/",
    name: "home",
  },
  {
    path: "/ingredientView",
    name: "ingredientsView",
    component: () => import("@/views/IngredientsView.vue")
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
});


export default router
