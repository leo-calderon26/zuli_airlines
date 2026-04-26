<script setup>

</script>

<template>
    <header class="header">
        <h1>aeronaves</h1>
        <nav>
            <ul class="nav_list">
                <li>
                    <router-link class="link" to="/">home</router-link>
                </li>
                <li>
                    <router-link class="link" to="/aircraft">Aeronaves</router-link>
                </li>
                <li>
                    <router-link class="link" :to= "{ name: 'createAircraft' }">Crear aeoronaves</router-link>
                </li>
            </ul>
        </nav>
    </header>
</template>

<style>
    .header {
        background-color: rgb(0, 0, 0);
        color: white;
        text-align: center;
        padding: 20px;
    }
    .nav_list {
        display: flex;
        justify-content: space-around;
        list-style: none;
    }
    .link {
        color: white;
        text-decoration: none;
    }
    .login-bar {
        width: 100%;
        position: absolute;
        top: 10px;
        right: 10px;
        display: flex;
        justify-content: end;
    }
    .username{
        margin: 0;
        padding-right: 10px;
    }
</style>