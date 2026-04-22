import { createRouter, createWebHistory } from 'vue-router'
import MainMenu from '../src/views/MainMenu.vue'
import Aircrafts from '../src/views/Aircrafts.vue'
import Airports from '../src/views/Airports.vue'
import Routes from '../src/views/Routes.vue'
import Users from '../src/views/Users.vue'
import Reports from '../src/views/Reports.vue'
import ProfileSettings from '../src/views/ProfileSettings.vue'

const routes = [
    { path: '/', name: 'main-menu', component: MainMenu },
    { path: '/aircrafts', name: 'aircrafts', component: Aircrafts },
    { path: '/airports', name: 'airports', component: Airports },
    { path: '/routes', name: 'routes', component: Routes },
    { path: '/users', name: 'users', component: Users },
    { path: '/reports', name: 'reports', component: Reports },
    { path: '/profile-settings', name: 'profileSettings', component: ProfileSettings }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router