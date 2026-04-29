import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
<<<<<<< HEAD
import router from '../router'

createApp(App).use(router).mount('#app')
=======
import router from './router/'
import { createPinia } from 'pinia'

const pinia = createPinia();

createApp(App).use(router).use(pinia).mount('#app')
>>>>>>> origin/feature/aircraft/create-view
