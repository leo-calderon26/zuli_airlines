
<template>
    <PublicNavBar />
        <nav class="bg-neutral-primary-soft border-default">
            <div class="flex">
            <div class="size-14 grow-2"></div>
                <div id="menu" class="bg-neutral-primary-soft border-purple shadow-xs border-y my-77">
                <div class="flex items-center justify-center sm:items-stretch px-4 py-5 mx-auto text-heading sm:grid-cols-2 md:grid-cols-3 ">
                    <ul aria-labelledby="menu-button">
                        <li class="hover:bg-purple">
                            <router-link to="/admin/aircrafts"><img src="../../../assets/aircraft.png" alt="Aircraft Icon" class="menuIcons" /></router-link>
                            <a href="#" class="block p-3 rounded-lg hover:bg-neutral-secondary-medium">
                                <div class="font-semibold text-gold text-center">Aeronaves</div>
                            </a>
                        </li>
                        <li class="hover:bg-purple">
                            <router-link to="/admin/airports"><img src="../../../assets/airport.png" alt="Airport Icon" class="menuIcons" /></router-link>
                            <a href="#" class="block p-3 rounded-lg hover:bg-neutral-secondary-medium">
                                <div class="font-semibold text-gold text-center">Aeropuertos</div>
                            </a>
                        </li>
                        <li class="hover:bg-purple">
                            <router-link to="/admin/routes"><img src="../../../assets/route.png" alt="Route Icon" class="menuIcons" /></router-link>
                            <a href="#" class="block p-3 rounded-lg hover:bg-neutral-secondary-medium">
                                <div class="font-semibold text-gold text-center">Rutas</div>
                            </a>
                        </li>
                        <li class="hover:bg-purple">
                            <router-link to="/admin/flights"><img src="../../../assets/flight.png" alt="User Icon" class="menuIcons" /></router-link>
                            <a href="#" class="block p-3 rounded-lg hover:bg-neutral-secondary-medium">
                                <div class="font-semibold text-gold text-center">Vuelos</div>
                            </a>
                        </li>
                        <li class="hover:bg-purple">
                            <router-link to="/admin/users"><img src="../../../assets/user.png" alt="User Icon" class="menuIcons" /></router-link>
                            <a href="#" class="block p-3 rounded-lg hover:bg-neutral-secondary-medium">
                                <div class="font-semibold text-gold text-center">Usuarios</div>
                            </a>
                        </li>
                        <li class="hover:bg-purple">
                            <router-link to="/admin/users"><img src="../../../assets/report.png" alt="User Icon" class="menuIcons" /></router-link>
                            <a href="#" class="block p-3 rounded-lg hover:bg-neutral-secondary-medium">
                                <div class="font-semibold text-gold text-center">Reportes</div>
                            </a>
                        </li>
                    </ul>
                </div>
                </div>
            <div class="size-14 grow-2"></div>
            </div>
        </nav>
    <PublicBottomBar />
</template>

<script>
import PublicNavBar from '../../../components/PublicNavBar.vue';
import PublicBottomBar from '../../../components/PublicBottomBar.vue';
    export default {
        components: {
            PublicNavBar,
            PublicBottomBar
        }
    }
</script>

<style scoped>
    .background {
        float: none;
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        background-color: white;
        width: 1000px;
        height: 400px;
        border-radius: 25px;
    }
    .menuIcons {
        width: 120px;
        height: clamp(48px, 8vw, 100px);
    }
    .menuOptions {
        float: none;
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        border-top: 2px solid rgba(232, 223, 247);
        border-bottom: 2px solid rgba(232, 223, 247);
    }
    .nav-links a:hover {
        background-color: #f2f0f5;
        border-radius: 15px;
    }

.landing-page {
  min-height: 100svh;
  display: flex;
  flex-direction: column;
}

.landing-page > :last-child {
  margin-top: auto;
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

    .nav-links {
        color: #8b953f;
        text-decoration: none;
        font-weight: 600;
    }

    .nav-links a {
        color: #8b953f;
        text-decoration: none;
        font-weight: 600;
        padding: 10px 14px;
        display: inline-block;
    }

.nav-links a.router-link-exact-active {
    background-color: rgba(0, 0, 0, 0.25);
    border-radius: 6px;
}

</style>