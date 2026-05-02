import { createRouter, createWebHistory } from "vue-router";
import FlightList from "../modules/flight/view/FlightList.vue"
import AircraftList from "../modules/aircraft/view/AircraftList.vue";
import AircraftCreate from "../modules/aircraft/view/AircraftCreate.vue"

import ReserveView from '../modules/landing/view/ReserveView.vue'
import CheckInView from '../modules/landing/view/CheckInView.vue'
import ConsultView from '../modules/landing/view/ConsultView.vue'
import HelpView from '../modules/landing/view/HelpView.vue'

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: "/",
            name: "reserve",
            component: ReserveView
        },
        {
            path: "/flights",
            name: "home",
            component: FlightList
        },
        {
            path: "/aircraft",
            name: "aircraftList",
            component: AircraftList
        },
        {
            path: "/create-aircraft",
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
            path: "/administrativo",
            name: "administrativo",
            component: HelpView
        }
    ]
});

export default router;