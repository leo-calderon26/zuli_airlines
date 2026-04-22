import { createRouter, createWebHistory } from 'vue-router'
import MainMenu from '../src/views/MainMenu.vue'
import Aircrafts from '../src/views/Aircrafts.vue'
import Airports from '../src/views/Airports.vue'
import Routes from '../src/views/Routes.vue'
import Users from '../src/views/Users.vue'

const routes = [
    { path: '/', name: 'main-menu', component: MainMenu },
    { path: '/aircrafts', name: 'aircrafts', component: Aircrafts },
    { path: '/airports', name: 'airports', component: Airports },
    { path: '/routes', name: 'routes', component: Routes },
    { path: '/users', name: 'users', component: Users }
    // { path: '/Reports', name: 'reports', component: Reports }
    // { path: '/ProfileSettings', name: 'profileSettings', component: profileSettings }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router