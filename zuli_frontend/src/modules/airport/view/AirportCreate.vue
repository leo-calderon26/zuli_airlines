<script setup>
import PublicNavBar from '../../../shared/PublicNavBar.vue';
import PublicBottomBar from '../../../shared/PublicBottomBar.vue';
import AirportForm from '../components/AirportForm.vue';
import AirportNavBar from '../components/AirportNavBar.vue';
</script>

<template>
    <div class="flex flex-col min-h-screen">
        <PublicNavBar/>   
        <AirportNavBar/>
        <div class="flex-grow py-8 px-4">
            <AirportForm/>
        </div>
        <PublicBottomBar />
    </div>
</template>