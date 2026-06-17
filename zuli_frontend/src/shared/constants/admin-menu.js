import cash from '../components/assets/cash.png'
import flightsIcon from '../components/assets/flight-mode.png'

export const REPORT_MENU_ITEMS = [
  { 
    title: 'Ingresos',
    icon: cash, 
    route: '/admin/reports/income' 
  },
  { 
    title: 'Vuelos', 
    icon: flightsIcon,
    route: '/admin/reports/flights' 
  }
]