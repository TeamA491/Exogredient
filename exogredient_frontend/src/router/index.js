import Vue from 'vue';
import VueRouter from 'vue-router';
Vue.use(VueRouter);

const routes = [
    {
        name: 'search',
        path: '/search',
        component: () => import('../views/SearchResultsView.vue'),
    },
    {
        name: 'store',
        path: '/store',
        component: () => import('../views/StoreView.vue'),
    },
    {
        name: 'error',
        path: '/error',
        component: () => import('../views/ErrorView'),
    },
    {
        name: 'register',
        path: '/register',
        component: () => import('../views/RegistrationView.vue'),
    },
    {
        path: '/pageNotFound',
        name: 'pageNotFound',
        component: () => import('../views/InvalidPage.vue'),
    },
    {
        path: '/resetPassword/:token',
        name: 'resetPassword',
        component: () => import('../views/ResetPasswordView.vue'),
        props: true,
    },
    {
        path: '/sendResetLink',
        name: 'sendResetLink',
        component: () => import('../views/SendResetLink.vue'),
    },
    {
        path: '/verify',
        name: 'verify',
        component: () => import('../views/VerificationView.vue'),
    },
    {
        path: '/login',
        name: 'login',
        component: () => import('../views/LoginView.vue'),
    },
    {
        path: '/profile',
        name: 'profile',
        component: () => import('@/views/UserProfile.vue'),
    },
    {
        path: '/upload',
        name: 'upload',
        component: () => import('@/views/Upload.vue'),
    },
    {
        path: '/',
        name: 'home',
    },
    {
        path: '/ingredientView',
        name: 'ingredientView',
        component: () => import('@/views/IngredientsView.vue'),
    },
    {
        path: '/useranalysis',
        name: 'useranalysis',
        component: () => import('@/views/UserAnalysis.vue'),
    },
];

const router = new VueRouter({
    base: process.env.BASE_URL,
    routes,
});

export default router;
