import { createRouter, createWebHistory } from "vue-router";

import FlightList from "../modules/flight/view/FlightList.vue"

import AircraftList from "../modules/aircraft/view/AircraftList.vue";
import AircraftCreate from "../modules/aircraft/view/AircraftCreate.vue"

import Login from"../modules/auth/view/Login.vue"
import authService from "../modules/auth/services/authService";

import ReserveView from '../modules/landing/view/ReserveView.vue'
import CheckInView from '../modules/landing/view/CheckInView.vue'
import ConsultView from '../modules/landing/view/ConsultView.vue'
import HelpView from '../modules/landing/view/HelpView.vue'

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: "/admin/",
            name: "reserve",
            component: ReserveView
        },
        {
            path: "/flights",
            name: "mainMenu",
            component: MainMenu
        },
        {
            path: "/admin/aircrafts/",
            name: "aircraftList",
            component: AircraftList
        },
        {
            path: "/admin/create-aircraft",
            name: "createAircraft",
            component: AircraftCreate
        },
        {
            path: "/admin/airports",
            name: "airports",
            component: Airports
        },
        {
            path: "/admin/routes",
            name: "routes",
            component: Routes
        },
         {
            path: "/admin/flights",
            name: "flights",
            component: Flights
        },
        {
            path: "/admin/users",
            name: "users",
            component: Users
        },
        {
            path: "/admin/reports",
            name: "reports",
            component: Reports
        },
        {
            path: "/admin/profile-settings",
            name: "profileSettings",
            component: ProfileSettings
        },
    ]
});

router.beforeEach(async (to, from, next) => {
    if (!to.meta.requiresAuth) {
        next();
        return;
    }

    try {
        await authService.me();
        next();
    } catch {
        next({ name: "administrativo" });
    }
});

export default router;