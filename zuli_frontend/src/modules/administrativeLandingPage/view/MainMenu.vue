
<template>
    <div class="flex flex-col">
            <nav class="bg-neutral-primary-soft border-default">
                <div class="flex">
                <div class="size-14 grow-2"></div>
                    <div id="menu" class="bg-neutral-primary-soft border-purple shadow-xs border-y my-77">
                    <div class="flex items-center justify-center sm:items-stretch px-4 py-5 mx-auto text-heading sm:grid-cols-2 md:grid-cols-3 ">
                        <ul aria-labelledby="menu-button">
                            <li class="hover:bg-purple">
                                <router-link to="/admin/aircrafts" class="block p-3 rounded-lg hover:bg-neutral-secondary-medium"><img :src="aircraftIcon" alt="Aircraft Icon" class="menuIcons" />
                                    <div class="font-semibold text-gold text-center">Aeronaves</div>
                                </router-link>
                            </li>
                            <li class="hover:bg-purple">
                                <router-link to="/admin/airports" class="block p-3 rounded-lg hover:bg-neutral-secondary-medium"><img :src="airportIcon" alt="Airport Icon" class="menuIcons" />
                                    <div class="font-semibold text-gold text-center">Aeropuertos</div>
                                </router-link>
                            </li>
                            <li class="hover:bg-purple">
                                <router-link to="/admin/routes" class="block p-3 rounded-lg hover:bg-neutral-secondary-medium"><img :src="routeIcon" alt="Route Icon" class="menuIcons" />
                                    <div class="font-semibold text-gold text-center">Rutas</div>
                                </router-link>
                            </li>
                            <li class="hover:bg-purple">
                                <router-link to="/admin/users" class="block p-3 rounded-lg hover:bg-neutral-secondary-medium"><img :src="userIcon" alt="User Icon" class="menuIcons" />
                                    <div class="font-semibold text-gold text-center">Usuarios</div>
                                </router-link>
                            </li>
                            <li class="hover:bg-purple">
                                <router-link to="/admin/reports" class="block p-3 rounded-lg hover:bg-neutral-secondary-medium"><img :src="reportIcon" alt="User Icon" class="menuIcons" />
                                    <div class="font-semibold text-gold text-center">Reportes</div>
                                </router-link>
                            </li>
                        </ul>
                    </div>
                    </div>
                <div class="size-14 grow-2"></div>
                </div>
            </nav>
    </div>
</template>

<script>
import aircraftIcon from '../../../assets/Aircraft.png';
import airportIcon from '../../../assets/Airport.png';
import routeIcon from '../../../assets/Route.png';
import flightIcon from '../../../assets/Flight.png';
import userIcon from '../../../assets/User.png';
import reportIcon from '../../../assets/Report.png';

export default {
    components: {
    },
    data() {
        return {
            aircraftIcon,
            airportIcon,
            routeIcon,
            flightIcon,
            userIcon,
            reportIcon
        };
    }
}
</script>

<style scoped>
    .menuIcons {
        width: 120px;
        height: clamp(48px, 8vw, 100px);
    }

nav ul {
    display: flex;
    align-items: center;
    list-style: none;
    margin: 0;
    padding: 0;
}

nav ul li {
    padding: 0 18px;
}

    nav ul li + li {
        border-left: 2px solid rgba(232, 223, 247);
        border-left-style: dashed;
    }

</style>