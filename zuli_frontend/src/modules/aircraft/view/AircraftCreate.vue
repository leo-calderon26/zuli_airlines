<script setup>
import PublicNavBar from '../../../shared/PublicNavBar.vue';
import AircraftForm from '../components/AircraftForm.vue';
import AircraftNavBar from '../components/AircraftNavBar.vue';

</script>

<template>
    <div class="flex flex-col">
        <PublicNavBar/>   
        <AircraftNavBar/>
        <main class="flex-1 pb-8">
            <div class="page-shell">
                <AircraftForm/>
            </div>
        </main>
    </div>
</template>

<style>
</style>