import { createRouter, createWebHistory } from "vue-router";
import Flights from "../modules/flight/view/FlightList.vue"

import FlightList from "../modules/flight/view/FlightList.vue"

import AircraftList from "../modules/aircraft/view/AircraftList.vue";
import AircraftCreate from "../modules/aircraft/view/AircraftCreate.vue"
import MainMenu from '../modules/administrativeLandingPage/view/MainMenu.vue'
import Airports from '../modules/airport/view/Airports.vue'
import Routes from '../modules/route/view/Routes.vue'
import Users from '../modules/user/view/Users.vue'
import ProfileSettings from '../modules/profile/view/ProfileSettings.vue'
import Reports from '../modules/reports/view/Reports.vue'

import ReserveView from '../modules/landing/view/ReserveView.vue'
import CheckInView from '../modules/landing/view/CheckInView.vue'
import ConsultView from '../modules/landing/view/ConsultView.vue'
import HelpView from '../modules/landing/view/HelpView.vue'
import Login from '../modules/auth/view/Login.vue'


const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: "/",
            name: "reserve",
            component: ReserveView
        },
        {
            path: "/admin/",
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
            path: "/check-in",
            name: "checkin",
            component: CheckInView
        },
        {
            path: "/consulta",
            name: "consult",
            component: ConsultView
        },
        {
            path: "/ayuda",
            name: "help",
            component: HelpView
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
        {
            path: "/administrativo",
            name: "administrativo",
            component: Login
        }
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