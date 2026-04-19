<<<<<<< HEAD
import { createRouter, createWebHistory } from "vue-router";
import Login from "../src/views/Login.vue";

const routes = [
    {
        path: "/",
        redirect: "/login"
    },
    {
        path: "/login",
        name: "Login",
        component: Login
    }
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;
=======
import { createRouter, createWebHistory } from 'vue-router'
import LPReserve from '../src/views/LPReserve.vue'
import LPCheckIn from '../src/views/LPCheckIn.vue'
import LPConsult from '../src/views/LPConsult.vue'
import LPHelp from '../src/views/LPHelp.vue'

const routes = [
  { path: '/', name: 'reserve', component: LPReserve },
  { path: '/Check-in', name: 'checkin', component: LPCheckIn },
  { path: '/Consulta', name: 'consult', component: LPConsult },
  { path: '/Ayuda', name: 'help', component: LPHelp }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
>>>>>>> 998718c (Lnading page con routing funcional y BottomBar)
