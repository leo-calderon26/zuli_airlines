<script setup>
import PublicNavBar from '../../../shared/PublicNavBar.vue';
import PublicBottomBar from '../../../shared/PublicBottomBar.vue';
import AirportForm from '../components/AirportForm.vue';
import AirportNavBar from '../components/AirportNavBar.vue';
</script>

<template>
    <div class="flex flex-col">
        <PublicNavBar/>   
        <AirportNavBar/>
        <main class="flex-1 pb-8">
            <div class="page-shell">
                <AirportForm/>
            </div>
        </main>
    </div>
</template>