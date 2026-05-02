import { createRouter, createWebHistory } from "vue-router";
import FlightList from "../modules/flight/view/FlightList.vue"
import AircraftList from "../modules/aircraft/view/AircraftList.vue";
import AircraftCreate from "../modules/aircraft/view/AircraftCreate.vue"

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: "/",
            name: "home",
            component: FlightList
        },
        {
            path: "/aircraft/",
            name: "aircraftList",
            component: AircraftList
        },
        {
            path: "/create-aircraft",
            name: "createAircraft",
            component: AircraftCreate
        }
    ]
});

export default router;