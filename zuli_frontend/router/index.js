import { createRouter, createWebHistory } from 'vue-router'
import LPReserve from '../views/LPReserve.vue'
import LPCheckIn from '../views/LPCheckIn.vue'
import LPConsult from '../views/LPConsult.vue'
import LPHelp from '../views/LPHelp.vue'

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