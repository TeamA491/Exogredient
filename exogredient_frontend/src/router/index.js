import Vue from 'vue';
import VueRouter from 'vue-router';
import StoreView from '../views/StoreView';
import ErrorView from '../views/ErrorView';
import LoginView from '../views/LoginView';
import TicketsView from '../views/TicketsView';
import RegistrationView from '../views/RegistrationView';
import SearchResultsView from '../views/SearchResultsView';
import SubmitTicketView from '../views/SubmitTicketView.vue';

Vue.use(VueRouter);

const routes = [
    { path: '/SearchResultsView', component: SearchResultsView },
    { path: '/StoreView', component: StoreView },
    { path: '/ErrorView', component: ErrorView },
    { path: '/RegistrationView', component: RegistrationView },
    {
        path: '/verify',
        name: 'verify',
        component: () => import('../views/VerificationView.vue'),
    },
    {
        path: '/login/:afterRegistered',
        name: 'login',
        component: () => import('@/views/LoginView.vue'),
        props: true,
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
        path: '/viewTickets',
        name: 'tickets-view',
        component: TicketsView,
    },
    {
        path: '/newTicket',
        name: 'submit-ticket-view',
        component: SubmitTicketView,
    },
];

const router = new VueRouter({
    mode: 'history',
    base: process.env.BASE_URL,
    routes: routes,
});

export default router;
