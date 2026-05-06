<script setup>
import PublicNavBar from '../../../shared/PublicNavBar.vue';
import AirportComponent from '../components/AirportComponent.vue';
import AirportNavBar from '../components/AirportNavBar.vue';
</script>

<template>
    <div class="flex flex-col">
        <PublicNavBar/>
        <AirportNavBar/>
        <main class="flex-1 pb-8">
            <div class="page-shell">
                <AirportComponent/>
            </div>
        </main>
    </div>
</template>