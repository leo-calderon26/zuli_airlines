<script setup>
import AdminNavBar from '../../../shared/AdminNavBar.vue';
import AircraftForm from '../components/AircraftForm.vue';

</script>

<template>
    <div class="flex flex-col">
        <AdminNavBar/>
        <main class="flex-1 pb-8">
            <div class="page-shell">
                <AircraftForm/>
            </div>
        </main>
    </div>
</template>

<style>
</style>