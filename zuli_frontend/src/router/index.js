import { createRouter, createWebHistory } from "vue-router";
import Flights from "../modules/flight/view/FlightList.vue"
import authService from "../modules/auth/services/authService";
import FlightSearchResults from '../modules/landing/view/FlightSearchResults.vue';

import FlightList from "../modules/flight/view/FlightList.vue"

import AircraftList from "../modules/aircraft/view/AircraftList.vue";
import AircraftCreate from "../modules/aircraft/view/AircraftCreate.vue"
import AircraftEdit from "../modules/aircraft/view/AircraftEdit.vue"

import AirportList from "../modules/airport/view/AirportList.vue";
import AirportCreate from '../modules/airport/view/AirportCreate.vue'
import AirportEdit from '../modules/airport/view/AirportEdit.vue'

import MainMenu from '../modules/administrativeLandingPage/view/MainMenu.vue'
import Routes from '../modules/route/view/Routes.vue'
import RouteList from '../modules/route/view/RouteList.vue'

import ProfileSettings from '../modules/profile/view/ProfileSettings.vue'
import Reports from '../modules/reports/view/Reports.vue'
import IncomeReport from '../modules/reports/view/IncomeReportView.vue'
import FlightsReport from '../modules/reports/view/FlightsReportView.vue'
import FlightCreate from "../modules/flight/view/FlightCreate.vue"
import ReserveView from '../modules/landing/view/ReserveView.vue'
import CheckInView from '../modules/landing/view/CheckInView.vue'
import ConsultView from '../modules/landing/view/ConsultView.vue'
import HelpView from '../modules/landing/view/HelpView.vue'
import Login from '../modules/auth/view/Login.vue'
import BuyTicketView from '../modules/buyTicket/view/BuyTicketView.vue'

import UserList from "../modules/user/view/UserList.vue";
import UserCreate from "../modules/user/view/UserCreate.vue";
import UserEdit from "../modules/user/view/UserEdit.vue";
import UserActivation from "../modules/user/view/UserActivation.vue";

import PurchaseConfirmationView from "../modules/purchaseConfirmation/view/PurchaseConfirmationView.vue";


const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: "/",
            name: "reserve",
            component: ReserveView,
            meta: {requiresAuth: false, adminRequired: false}
        },
        {
            path: "/buscar-vuelos",
            name: "buscarVuelos",
            component: FlightSearchResults,
            meta: {requiresAuth: false, adminRequired: false}
        },
        {
            path: "/admin/",
            name: "mainMenu",
            component: MainMenu,
            meta: {requiresAuth: true, adminRequired: false}
        },
        {
            path: "/admin/aircrafts/",
            name: "aircraftList",
            component: AircraftList,
            meta: {requiresAuth: true, adminRequired: false}
        },
        {
            path: "/admin/create-aircraft",
            name: "createAircraft",
            component: AircraftCreate,
            meta: {requiresAuth: true, adminRequired: true}
        },
        {
            path: "/admin/edit-aircraft/:aircraftId",
            name: "editAircraft",
            component: AircraftEdit,
            meta: {requiresAuth: true, adminRequired: false}
        },
        {
            path: "/admin/flights",
            name: "flights",
            component: FlightCreate,
            meta: {requiresAuth: true, adminRequired: false}
        },
        {
            path: "/admin/create-flight",
            redirect: "/admin/flights"
        },
        {
            path: "/admin/create-airport",
            name: "createAirport",
            component: AirportCreate,
            meta: {requiresAuth: true, adminRequired: true}
        },
        {
            path: "/admin/edit-airport/:airportCode",
            name: "editAirport",
            component: AirportEdit,
            meta: {requiresAuth: true, adminRequired: false}
        },
        {
            path: "/check-in",
            name: "checkin",
            component: CheckInView,
            meta: {requiresAuth: false, adminRequired: false}
        },
        {
            path: "/consulta",
            name: "consult",
            component: ConsultView,
            meta: {requiresAuth: false, adminRequired: false}
        },
        {
            path: "/ayuda",
            name: "help",
            component: HelpView,
            meta: {requiresAuth: false, adminRequired: false}
        },
        {
            path: "/admin/airports",
            name: "airports",
            component: AirportList,
            meta: {requiresAuth: true, adminRequired: false}
        },
        {
            path: "/admin/routes",
            name: "routes",
            component: RouteList,
            meta: {requiresAuth: true, adminRequired: false}
        },
        {
            path: "/admin/create-route",
            name: "createRoute",
            component: Routes,
            meta: {requiresAuth: true, adminRequired: false}
        },
         {
            path: "/admin/flight-list",
            name: "flightList",
            component: FlightList,
            meta: {requiresAuth: true, adminRequired: false}
        },
        {
            path: "/admin/users",
            name: "user",
            component: UserList,
            meta: {requiresAuth: true, adminRequired: false}

        },
        {
            path: "/admin/create-user",
            name: "userCreate",
            meta: {requiresAuth: true, adminRequired: true},
            component: UserCreate
            
        },
        {
            path: "/admin/edit-user/:userId",
            name: "userEdit",
            component: UserEdit,
            meta: {requiresAuth: true, adminRequired: false}
        },
        {
            path: "/users/activation",
            name: "userActivation",
            component: UserActivation
        },
        {
            path: "/admin/reports",
            name: "reports",
            component: Reports,
            meta: {requiresAuth: true, adminRequired: false},
            children: [
                {
                    path: "income",
                    name: "incomeReport",
                    component: IncomeReport
                },
                {
                    path: "flights",
                    name: "flightsReport",
                    component: FlightsReport
                }
            ]
        },
        {
            path: "/admin/profile-settings",
            name: "profileSettings",
            component: ProfileSettings,
            meta: {requiresAuth: true, adminRequired: false}
        },
        {
            path: "/administrativo",
            name: "administrativo",
            component: Login,
            meta: {requiresAuth: false, adminRequired: false}
        },
        {
            path: "/comprar-boleto",
            name: "buyTicket",
            component: BuyTicketView,
            meta: {requiresAuth: false, adminRequired: false}
        },
        {
            path: "/purchase-confirmation/:reservationCode",
            name: "purchaseConfirmation",
            component: PurchaseConfirmationView,
            meta: { requiresAuth: false, adminRequired: false }
        }
    ]
});

router.beforeEach(async (to, from, next) => {
    if (to.meta.requiresAuth === false) {
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

router.beforeEach(async (to, from, next) => {
    if (to.meta.adminRequired === false) {
        next();
        return;
    }
    try {
        var data = await authService.me();
        if (data.userRole === 'Administrator') {
            next();
            return;
        }
        else
            next({ name: "unauthorizedAccess" });
    } catch {
        next({ name: "unauthorizedAccess" });
    }
});

export default router;