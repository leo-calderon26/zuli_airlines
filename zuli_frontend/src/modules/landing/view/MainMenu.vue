
<template>
    <div class="landing-page">
        <PublicNavBar />
        <div class="background">
        </div>
        <div class="menuOptions">
            <nav class="nav-links">
                <ul>
                    <li><router-link to="/aircrafts"><img src="../assets/aircraft.png" alt="Aircraft Icon" class="menuIcons" />Aeronaves</router-link></li>
                    <li><router-link to="/airports"><img src="../assets/airport.png" alt="Airport Icon" class="menuIcons" />Aeropuertos</router-link></li>
                    <li><router-link to="/routes"><img src="../assets/route.png" alt="Route Icon" class="menuIcons" />Rutas</router-link></li>
                    <li><router-link to="/users"><img src="../assets/user.png" alt="User Icon" class="menuIcons" />Usuarios</router-link></li>
                    <li><router-link to="/reports"><img src="../assets/report.png" alt="Report Icon" class="menuIcons" />Reportes</router-link></li>
                </ul>
            </nav>
        </div>
        <PublicBottomBar />
    </div>
</template>

<script>
import PublicNavBar from '../components/PublicNavBar.vue';
import PublicBottomBar from '../../../shared/PublicBottomBar.vue';
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